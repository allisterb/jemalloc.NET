using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace jemalloc
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public readonly struct FixedBuffer<T> : IDisposable, IRetainable, IEquatable<FixedBuffer<T>>, IEnumerable<T> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        #region Constructors
        public FixedBuffer(int length)
        {
            _Ptr = IntPtr.Zero;
            _Length = 0;
            _SizeInBytes = 0;
            _Timestamp = 0;
            IsReadOnly = false;
            AllocateThreadId = 0;
            Rid = JemUtil.Rng.Next(0, 4096);
            ThrowIfTypeNotPrimitive();
            long t = DateTime.UtcNow.Ticks;
            int th = Thread.CurrentThread.ManagedThreadId;
            _Ptr = Jem.AllocateFixedBuffer<T>((ulong)length, ElementSizeInBytes, t, th, Rid);
            if (_Ptr != IntPtr.Zero)
            {
                _Length = length;
                _SizeInBytes = (ulong)_Length * ElementSizeInBytes;
                _Timestamp = t;
                AllocateThreadId = th;
                
            }
            else throw new OutOfMemoryException($"Could not allocate {(ulong)_Length * ElementSizeInBytes} bytes for {Name}");

        }

        public FixedBuffer(int length, bool isReadOnly) : this(length)
        {
            IsReadOnly = true;

        }
        public FixedBuffer(T[] array) : this(array.Length)
        {
            ReadOnlySpan<T> arraySpan = new ReadOnlySpan<T>(array);
            arraySpan.CopyTo(this.WriteSpan);
        }

        public FixedBuffer(Span<T> span) : this(span.Length)
        {
            span.CopyTo(this.WriteSpan);
        }
        public FixedBuffer(ReadOnlySpan<T> span) : this(span.Length)
        {
            IsReadOnly = true;
            span.CopyTo(this.WriteSpan);
        }
        #endregion

        #region Implemented members
        public void Retain()
        {
            ThrowIfInvalid();
            Jem.IncrementRefCount(_Ptr);
        }
        public bool Release()
        {
            ThrowIfInvalid();
            if (RefCount == 0)
            {
                return false;
            }
            else
            {
                Jem.DecrementRefCount(_Ptr);
                return true;
            }
         }

        bool IEquatable<FixedBuffer<T>>.Equals(FixedBuffer<T> buffer)
        {
            return this._Ptr == buffer.Ptr && this.Length == buffer.Length && this._Timestamp == buffer.Timestamp 
                && this.AllocateThreadId == buffer.AllocateThreadId && this.Rid == buffer.Rid;
        }

        public IEnumerator<T> GetEnumerator() => new FixedBufferEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => new FixedBufferEnumerator<T>(this);

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

        #region Public
        public bool IsInvalid => _Ptr == IntPtr.Zero || !Jem.FixedBufferIsAllocatedWith(_Ptr, _SizeInBytes, _Timestamp, AllocateThreadId, Rid);

        public bool IsValid => !IsInvalid;

        public int RefCount
        {
            get
            {
                ThrowIfInvalid();
                return Jem.GetRefCount(_Ptr);
            }
        }

        public bool IsRetained => RefCount > 0;

        public bool IsReadOnly { get; }

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

        public unsafe ReadOnlySpan<T> Span
        {
            get
            {
                ThrowIfInvalid();
                return new ReadOnlySpan<T>(_Ptr.ToPointer(), _Length);
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
        #endregion

        internal long Timestamp
        {
            get
            {
                ThrowIfInvalid();
                return _Timestamp;
            }
        }
        internal unsafe Span<T> WriteSpan
        {
            get
            {
                ThrowIfInvalid();
                return new Span<T>(_Ptr.ToPointer(), _Length);
            }
        }


        #endregion

        #region Methods
        public void Acquire() => Retain();

        public bool Free()
        {
            if (IsInvalid)
            {
                return false;
            }
            if(IsRetained)
            {
                return false;
            }
            IntPtr p = _Ptr;
       
            if (Interlocked.Exchange(ref p, IntPtr.Zero) != IntPtr.Zero)
            {
                if (Jem.FreeFixedBuffer(_Ptr))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Fill(T value)
        {
            Retain();
            WriteSpan.Fill(value);
            Release();
        }

        public void CopyTo(T[] array)
        {
            AcquireSpan().CopyTo(new Span<T>(array));
            Release();
        }

        public T[] CopyToArray()
        {
            T[] array = new T[Length];
            AcquireSpan().CopyTo(new Span<T>(array));
            Release();
            return array;
        }

        public T[] FreeAndCopyToArray()
        {
            T[] array = CopyToArray();
            Free();
            return array;
        }

        public bool EqualTo(T[] array)
        {
            if (_Length != array.Length)
            {
                return false;
            }
            else
            {
                Retain();
                ReadOnlySpan<T> span = new ReadOnlySpan<T>(array);
                bool ret = this.WriteSpan.SequenceEqual(span);
                Release();
                return ret;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<T> AcquireSpan()
        {
            Acquire();
            return new ReadOnlySpan<T>((void*) _Ptr, (int)Length);
        }


        public ReadOnlySpan<T> Slice(int start, int length)
        {
            return Span.Slice(start, length);
        }

        public Vector<T> AcquireAsSingleVector()
        {
            if (this.Length != Vector<T>.Count)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<T>.Count} elements to create a vector of type {JemUtil.CLRType<T>().Name}.");
            }
            ReadOnlySpan<T> span = AcquireSpan();
            ReadOnlySpan<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            return vector[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<Vector<T>> AcquireVectorSpan()
        {
            ThrowIfNotVectorizable();
            return AcquireSpan().NonPortableCast<T, Vector<T>>();
        }

        #region Arithmetic
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorMultiply(T value)
        {
            ThrowIfNotVectorizable();
            T[] fill = new T[JemUtil.VectorLength<T>()];
            Span<T> fillSpan = new Span<T>(fill);
            fillSpan.Fill(value);
            Span<Vector<T>> vectorSpan = AcquireVectorWriteSpan();
            Vector<T> mulVector = fillSpan.NonPortableCast<T, Vector<T>>()[0];
            for (int i = 0; i < vectorSpan.Length; i++)
            {
                vectorSpan[i] = Vector.Multiply(vectorSpan[i], mulVector);
            }
            Release();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorSqrt()
        {
            ThrowIfNotVectorizable();
            Span<Vector<T>> vector = AcquireVectorWriteSpan();
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = Vector.SquareRoot(vector[i]);
            }
            Release();
        }

        #endregion

        public unsafe Span<T> AcquireWriteSpan()
        {
            Acquire();
            return new Span<T>((void*)_Ptr, _Length);
        }

        public unsafe Span<Vector<T>> AcquireVectorWriteSpan()
        {
            ThrowIfNotVectorizable();
            return AcquireWriteSpan().NonPortableCast<T, Vector<T>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ref T Read(int index)
        {
            //Retain();
            // return (T*) (_ptr + byteOffset);
            ref T ret = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            //Release();
            return ref ret;
        }


        internal unsafe ref T Write(int index, ref T value)
        {
            Retain();
            ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            v = value;
            Release();
            return ref v;
        }

        
        internal void ThrowIfInvalid()
        {
            if (IsInvalid)
            {
                throw new InvalidOperationException($"{nameof(FixedBuffer<T>)}({this._Length}) is invalid.");
            }
        }

        internal void ThrowIfIndexOutOfRange(int index)
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

        internal void ThrowIfRefCountNonZero()
        {
            if (0 > 0)
            {
                throw new InvalidOperationException($"{nameof(FixedBuffer<T>)}({this._Length}) has RefCount .");
            }
        }

        internal void ThrowIfReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException($"{nameof(FixedBuffer<T>)}({this._Length}) is read-only.");
            }
        }

        internal void ThrowIfTypeNotPrimitive()
        {
            if (!typeof(T).IsPrimitive)
            {
                throw new ArgumentException($"The type {typeof(T).Name} is not a primitive type.");
            }
        }

        internal void ThrowIfNotVectorizable()
        {
            if (Length == 0 || Length % JemUtil.VectorLength<T>() != 0)
            {
                throw new InvalidOperationException("Buffer is not vectorizable.");
            }
        }

        internal string Name => $"{nameof(FixedBuffer<T>)}({this._Length})";

        private string DebuggerDisplay() => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion

        #region Operators
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this.Read(index);
        }
        #endregion

        #region Fields
        private static readonly Type ElementType = typeof(T);
        private static readonly ulong ElementSizeInBytes = (ulong) JemUtil.SizeOfStruct<T>();
        private readonly IntPtr _Ptr;
        private readonly ulong _SizeInBytes;
        private readonly int _Length;
        private readonly long _Timestamp;
        internal readonly int AllocateThreadId;
        internal readonly int Rid;
        #endregion
    }
}
