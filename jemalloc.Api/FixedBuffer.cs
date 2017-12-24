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
            _Timestamp = DateTime.UtcNow.Ticks;
            RefCount = 0;
            IsReadOnly = false;
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

        public unsafe FixedBuffer(Span<T> span) : this(span.Length)
        {
            span.CopyTo(this.Span);
        }
        public unsafe FixedBuffer(ReadOnlySpan<T> span) : this(span.Length)
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
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) has outstanding references.");
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
        public bool IsInvalid => _Ptr == IntPtr.Zero || !Jem.FixedBufferIsAllocatedWith(_Ptr, _SizeInBytes, _Timestamp);

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
            _Ptr = Jem.CallocFixedBuffer<T>((ulong)length, ElementSizeInBytes, _Timestamp);
            if (_Ptr != IntPtr.Zero)
            {
                _Length = length;
                _SizeInBytes = (ulong)_Length * ElementSizeInBytes;
                return true;
            }
            else return false;
        }

        public unsafe MemoryHandle Acquire()
        {
            ThrowIfInvalid();
            ThrowIfRefCountNonZero();
            RefCount++;
            return new MemoryHandle(this, this.Ptr.ToPointer());
        }

        public void Release()
        {
            ThrowIfInvalid();
            ThrowIfRefCountNonZero();
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
            ThrowIfInvalid();
            Span.Fill(value);
        }

        public bool EqualTo(T[] array)
        {
            if (_Length != array.Length)
            {
                return false;
            }
            else
            {
                ReadOnlySpan<T> span = new ReadOnlySpan<T>(array);
                return this.Span.SequenceEqual(span);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T Read(int index)
        {
            ThrowIfInvalid();
            Acquire();
            // return (T*) (_ptr + byteOffset);
            T ret = Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            Release();
            return ret;
        }


        private unsafe T Write(int index, T value)
        {
            ThrowIfInvalid();
            ThrowIfReadOnly();
            Acquire();
            ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            v = value;
            Release();
            return value;
        }

        private void ThrowIfInvalid()
        {
            if (IsInvalid)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) is invalid.");
            }
        }

        private void ThrowIfRefCountNonZero()
        {
            if (RefCount > 0)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) has RefCount {RefCount}.");
            }
        }

        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) is read-only.");
            }
        }

        #endregion

        #region Operators
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (IsInvalid)
                    throw new InvalidOperationException("The buffer is invalid.");
                if (index >= (_Length))
                    throw new IndexOutOfRangeException();
                unsafe
                {
                    return ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
                }
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
         
        #endregion
    }
}
