using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace jemalloc
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FixedBuffer<T> : IDisposable, IRetainable, IEquatable<FixedBuffer<T>> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        #region Constructors
        public FixedBuffer(int length)
        {
            _Ptr = IntPtr.Zero;
            _Length = 0;
            _SizeInBytes = 0;
            _Timestamp = 0;
            RefCount = 0;
            IsReadOnly = false;
            AllocateThreadId = 0;
            ThrowIfTypeNotPrimitive();
            Allocate(length);
        }

        public FixedBuffer(int length, bool isReadOnly) : this(length)
        {
            IsReadOnly = true;

        }
        public FixedBuffer(T[] array) : this(array.Length)
        {           
            ReadOnlySpan<T> arraySpan = new ReadOnlySpan<T>(array);
            arraySpan.CopyTo(this.Span);   
        }

        public FixedBuffer(Span<T> span) : this(span.Length)
        {
            span.CopyTo(this.Span);
        }
        public FixedBuffer(ReadOnlySpan<T> span) : this(span.Length)
        {
            IsReadOnly = true;
            span.CopyTo(this.Span);
        }
        #endregion

        #region Implemented members
        void IRetainable.Retain() => ++RefCount;
        bool IRetainable.Release() => Free();
        bool IEquatable<FixedBuffer<T>>.Equals(FixedBuffer<T> buffer)
        {
            return this._Ptr == buffer.Ptr && this.Length == buffer.Length && this._Timestamp == buffer.Timestamp;
        }
        #region Disposer
        void IDisposable.Dispose()
        {
            if (IsRetained)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T)}[{_Length}] has outstanding references.");
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Free();
        }

        #endregion

        #endregion

        #region Properties
        public bool IsInvalid => _Ptr == IntPtr.Zero || !Jem.FixedBufferIsAllocatedWith(_Ptr, _SizeInBytes, _Timestamp, AllocateThreadId);

        public bool IsRetained => RefCount > 0;

        public bool IsReadOnly { get; private set; }

        public int Length
        {
            get
            {
                ThrowIfInvalid();
                return _Length;
            }

        }

        public ulong Size
        {
            get
            {
                ThrowIfInvalid();
                return _SizeInBytes;
            }

        }

        internal IntPtr Ptr
        {
            get
            {
                ThrowIfInvalid();
                return _Ptr;
            }
        }

        internal long Timestamp
        {
            get
            {
                ThrowIfInvalid();
                return _Timestamp;
            }
        }
        public unsafe Span<T> Span
        {
            get
            {
                ThrowIfInvalid();
                return new Span<T>(_Ptr.ToPointer(), _Length);
            }
        }

        public unsafe ReadOnlySpan<T> ReadOnlySpan
        {
            get
            {
                ThrowIfInvalid();
                return new ReadOnlySpan<T>(_Ptr.ToPointer(), _Length);
            }
        }

        public int RefCount { get; internal set; }

        #endregion

        #region Methods
        private unsafe bool Allocate(int length)
        {
            long t = DateTime.UtcNow.Ticks;
            int th = Thread.CurrentThread.ManagedThreadId;
            _Ptr = Jem.AllocateFixedBuffer<T>((ulong)length, ElementSizeInBytes, t, th);
            if (_Ptr != IntPtr.Zero)
            {
                _Length = length;
                _SizeInBytes = (ulong)_Length * ElementSizeInBytes;
                _Timestamp = t;
                return true;
            }
            else return false;
        }

        public unsafe MemoryHandle Acquire()
        {
            ThrowIfInvalid();
            RefCount++;
            return new MemoryHandle(this, this.Ptr.ToPointer());
        }

        public void Release()
        {
            ThrowIfInvalid();
            RefCount--;
        }

        public bool Free()
        {
            ThrowIfInvalid();
            ThrowIfRefCountNonZero();
            if (Interlocked.Exchange(ref _Ptr, IntPtr.Zero) != IntPtr.Zero)
            {
                if (Jem.FreeFixedBuffer(_Ptr))
                {
                    _Ptr = IntPtr.Zero;
                    _Length = 0;
                    _SizeInBytes = 0;
                    _Timestamp = 0;
                    RefCount = 0;
                    IsReadOnly = false;
                    AllocateThreadId = 0;
                    return true;
                }
                else
                {
                    return false;
                    //throw new Exception($"Could not free buffer at pointer {_Ptr}.");
                }
            }
            else
            {
                return false;
            }
        }

        public void Fill(T value)
        {
            Acquire();
            ThrowIfInvalid();
            Span.Fill(value);
            Release();
        }

        public bool EqualTo(T[] array)
        {
            if (_Length != array.Length)
            {
                return false;
            }
            else
            {
                Acquire();
                ReadOnlySpan<T> span = new ReadOnlySpan<T>(array);
                bool ret = this.Span.SequenceEqual(span);
                Release();
                return ret;
            }
        }

        public Span<T> AcquireSlice(int start, int length)
        {
            ThrowIfInvalid();
            Acquire();
            Span<T> ret = Span.Slice(start, length);
            return ret; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe ref T Read(int index)
        {
            ThrowIfInvalid();
            Acquire();
            // return (T*) (_ptr + byteOffset);
            ref T ret = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            Release();
            return ref ret;
        }


        private unsafe ref T Write(int index, T value)
        {
            ThrowIfInvalid();
            ThrowIfReadOnly();
            Acquire();
            ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            v = value;
            Release();
            return ref v;
        }

        
        private void ThrowIfInvalid()
        {
            if (IsInvalid)
            {
                throw new InvalidOperationException($"{nameof(FixedBuffer<T>)}({this._Length}) is invalid.");
            }
        }

        private void ThrowIfIndexOutOfRange(int index)
        { 
            if (index >= _Length)
            {
                throw new IndexOutOfRangeException($"Index {index} is greater than the maximum index of the buffer {_Length - 1}.");
            }
            else if (index < 0)
            {
                throw new IndexOutOfRangeException($"Index {index} is less than zero.");
            }
        }

        private void ThrowIfRefCountNonZero()
        {
            if (RefCount > 0)
            {
                throw new InvalidOperationException($"{nameof(FixedBuffer<T>)}({this._Length}) has RefCount {RefCount}.");
            }
        }

        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException($"{nameof(FixedBuffer<T>)}({this._Length}) is read-only.");
            }
        }

        private void ThrowIfTypeNotPrimitive()
        {
            if (!typeof(T).IsPrimitive)
            {
                throw new ArgumentException($"The type {typeof(T).Name} is not a primitive type.");
            }
        }

        #endregion

        #region Operators
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Read(index);
            }
            set
            {
                Write(index, value);
            }
        }
        #endregion

        #region Fields
        private static readonly Type ElementType = typeof(T);
        private static readonly ulong ElementSizeInBytes = (ulong) JemUtil.SizeOfStruct<T>();
        private IntPtr _Ptr;
        private ulong _SizeInBytes;
        private int _Length;
        private long _Timestamp;
        private int AllocateThreadId;
        
        #endregion
    }
}
