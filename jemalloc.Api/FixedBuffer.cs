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
            if (length == 0)
            {
                throw new ArgumentException("FixedBuffer Length cannot be zero.");
            }
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
            ThrowIfInvalid();
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
            return Jem.FreeFixedBuffer(_Ptr);
        }

        public void Fill(T value)
        {
            WriteSpan.Fill(value);
        }

        public void CopyTo(T[] array)
        {
            WriteSpan.CopyTo(new Span<T>(array));
        }

        public T[] CopyToArray()
        {
            T[] array = new T[Length];
            WriteSpan.CopyTo(new Span<T>(array));
            return array;
        }

        public T[] CopyToArray(T[] array)
        {
            if (_Length != array.Length)
            {
                throw new ArgumentException($"Array length {array.Length} is not the same as length {_Length} of {Name}.");
            }
            else
            {
                Span.CopyTo(new Span<T>(array));
                return array;
            }
        }

        public T[] CopyToArrayAndFree()
        {
            T[] array = CopyToArray();
            Free();
            return array;
        }

        public bool EqualTo(T[] array)
        {
            ThrowIfInvalid();
            if (_Length != array.Length)
            {
                return false;
            }
            else
            {
                ReadOnlySpan<T> span = new ReadOnlySpan<T>(array);
                bool ret = this.Span.SequenceEqual(span);
                return ret;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<T> AcquireSpan()
        {
            Acquire();
            return new ReadOnlySpan<T>((void*) _Ptr, (int)Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<C> AcquireSpan<C>() where C : struct, IEquatable<T>, IComparable<T>, IConvertible
        {
            int size = checked((int)(Size / (ulong)JemUtil.SizeOfStruct<C>()));
            if (size == 0)
            {
                throw new ArgumentException($"Type {typeof(T).Name} is too small to be reinterpreted as {typeof(C).Name}.");
            }
            else
            {
                Acquire();
                return new ReadOnlySpan<C>((void*)_Ptr, size);
            }
        }


        public ReadOnlySpan<T> Slice(int start, int length)
        {
            return Span.Slice(start, length);
        }

        public Vector<T> AcquireAsSingleVector()
        {
            if (this._Length != Vector<T>.Count)
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
            ThrowIfInvalid();
            return new ReadOnlySpan<Vector<T>>(_Ptr.ToPointer(), Length / JemUtil.VectorLength<T>());
        }
        
        public unsafe Span<T> AcquireWriteSpan()
        {
            Acquire();
            return new Span<T>((void*)_Ptr, _Length);
        }
        
        public unsafe Span<Vector<T>> AcquireVectorWriteSpan()
        {
            ThrowIfNotVectorizable();
            Acquire();
            return new Span<Vector<T>>(_Ptr.ToPointer(), _Length / JemUtil.VectorLength<T>());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ref T Read(int index)
        {
            ref T ret = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            return ref ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe C Read<C>(int index) where C : struct
        {
            ref T ret = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            return Unsafe.Read<C>(Unsafe.AsPointer(ref ret));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write<C>(int index, ref C value) where C : struct
        {
            ref T ret = ref Unsafe.Add(ref Unsafe.AsRef<T>(_Ptr.ToPointer()), index);
            Unsafe.Write(Unsafe.AsPointer(ref ret), value);
        }

        internal unsafe Span<Vector<T>> WriteVectorSpan
        {
            get
            {
                ThrowIfInvalid();
                return new Span<Vector<T>>(_Ptr.ToPointer(), _Length / JemUtil.VectorLength<T>());
            }
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
            if (!JemUtil.IsNumericType<T>() || _Length == 0 || (_Length % JemUtil.VectorLength<T>() != 0))
            {
                throw new InvalidOperationException("Buffer is not vectorizable.");
            }
        }

        internal string Name => $"{nameof(FixedBuffer<T>)}({this._Length})";

        private string DebuggerDisplay() => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);

        #region Arithmetic
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorFill(T value)
        {
            ThrowIfInvalid();
            int c = JemUtil.VectorLength<T>();
            int i;
            Vector<T> fill = new Vector<T>(value);
            Span<Vector<T>> s = WriteVectorSpan;
            for (i = 0; i < s.Length; i ++)
            {
                s[i] = fill;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorMultiply(T value)
        {
            ThrowIfInvalid();
            int c = JemUtil.VectorLength<T>();
            int i;
            T r;
            Vector<T> mul = new Vector<T>(value);
            for (i = 0; i < _Length - c; i += c)
            {
                Vector<T> f = Read<Vector<T>>(i);
                Vector<T> result = f * mul;
                Write(i, ref result);
            }
            
            for (; i < _Length; ++i)
            {
                r = GM<T>.Multiply(this[i], value);
                Write(i, ref r);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorSqrt()
        {
            ThrowIfInvalid();
            int c = JemUtil.VectorLength<T>();
            int i;
            T r;
            for (i = 0; i < _Length - c; i += c)
            {
                Vector<T> f = Read<Vector<T>>(i);
                Vector<T> result = Vector.SquareRoot(f);
                Write(i, ref result);
            }

            for (; i < _Length; ++i)
            {
                r = JemUtil.ValToGenericStruct<double, T>(GM<T>.Sqrt(this[i]));
                Write(i, ref r);
            }
        }

        public unsafe bool VectorLessThanAll(T value, out int index)
        {
            ThrowIfInvalid();
            index = -1;
            bool r = true;
            int c = Vector<T>.Count;
            int i;
            Vector<T> v = new Vector<T>(value);
            Vector<T> O = Vector<T>.One;
            for (i = 0; i <= _Length - c; i+= c)
            {
                Vector<T> s = Unsafe.Read<Vector<T>>(BufferHelpers.Add<T>(_Ptr, i).ToPointer());
                Vector<T> vcmp = Vector.LessThan(s, v);
                if (vcmp == O)
                {
                    continue;
                }
                else
                {
                    r = false;
                    for (int j = 0; j < c; j++)
                    {
                        if (vcmp[j].Equals(default))                      
                        {
                            index = i + j;
                            return r;
                        }
                    }
                    return r;
                }
            }

            for (; i < _Length; ++i)
            {
                if (Read(i).CompareTo(value) >= 0)
                {
                    r = false;
                    index = i;
                    return r;
                }
            }
            return r;
        }
            
        #endregion

        #endregion

        #region Operators
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this.Read(index);
        }

        public static implicit operator IntPtr (FixedBuffer<T> buffer)
        {
            return buffer._Ptr;
        }

       
        #endregion

        #region Fields
        private static readonly Type ElementType = typeof(T);
        private static readonly ulong ElementSizeInBytes = (ulong) JemUtil.SizeOfStruct<T>();
        private static readonly int VectorWidth = Vector<T>.Count;
        private readonly IntPtr _Ptr;
        private readonly ulong _SizeInBytes;
        private readonly int _Length;
        private readonly long _Timestamp;
        internal readonly int AllocateThreadId;
        internal readonly int Rid;
        #endregion
    }
}
