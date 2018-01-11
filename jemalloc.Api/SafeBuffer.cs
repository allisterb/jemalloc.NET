using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace jemalloc
{
    public abstract class SafeBuffer<T> : SafeHandle, IRetainable, IDisposable, IEquatable<SafeBuffer<T>>, IEnumerable<T> where T : struct, IEquatable<T>
    {
        #region Constructors
        protected SafeBuffer(int length, params T[] values) : base(IntPtr.Zero, true)
        {
            if (BufferHelpers.IsReferenceOrContainsReferences(typeof(T), out FieldInfo field))
            {
                throw new ArgumentException($"Only structures without reference fields can be used with this class. The field {field.Name} has type {field.FieldType.Name}.");
            }
            if (values.Length > length)
            {
                throw new ArgumentException("The length of the list of values must be smaller or equal to the length of the buffer");
            }
            SizeInBytes = NotAllocated;
            base.SetHandle(Allocate(length));
            if (IsAllocated)
            {
                CopyFrom(values);
            }
        }

        #endregion

        #region Overriden members
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            bool r = Jem.Free(handle);
            if (!r)
            {
                return false;
            }
            else
            {
                handle = IntPtr.Zero;
                unsafe
                {
                    voidPtr = (void *) 0;
                }
                Length = 0;
                SizeInBytes = 0;
                return true;
            }
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        #region Disposer
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThrowIfNotAllocatedOrInvalid();
                ThrowIfRetained();
                ReleaseHandle();
            }
            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Implemented members
        public void Retain() => Acquire();

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public bool Release()
        {
            
            if (IsNotAllocated || IsInvalid)
            {
                return false;
            }
            else
            {
                Jem.DecrementRefCount(handle);
                DangerousRelease();
                return true;
            }
        }

        public bool Equals(SafeBuffer<T> other)
        {
            ThrowIfNotAllocatedOrInvalid();
            return this.handle == other.handle && this.Length == other.Length;
        }

        public IEnumerator<T> GetEnumerator() => new SafeBufferEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => new SafeBufferEnumerator<T>(this);
        #endregion

        #region Properties
        public int Length { get; protected set; }

        public ulong SizeInBytes { get; protected set; }

        public bool IsNotAllocated => SizeInBytes == NotAllocated;

        public bool IsAllocated => !IsNotAllocated;

        public bool IsValid => !IsInvalid;

        public bool IsRetained => IsValid ? _RefCount > 0 : false;

        public bool IsVectorizable { get; protected set; }

        public Span<T> Span
        {
            get
            {
                ThrowIfNotAllocatedOrInvalid();
                return _Span;
            }
        }

        protected int _RefCount => Jem.GetRefCount(handle);

        protected unsafe Span<T> _Span => new Span<T>(voidPtr, Length);

        #endregion

        #region Methods

        #region Memory management
        protected unsafe virtual IntPtr Allocate(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");
            Contract.EndContractBlock();
            ulong s = checked((uint)length * ElementSizeInBytes);
            handle = Jem.Calloc((uint)length, ElementSizeInBytes);
            if (handle != IntPtr.Zero)
            {
                voidPtr = handle.ToPointer();
                Length = length;
                SizeInBytes = s;
                InitVector();
            }
            return handle;
        }

        protected void InitVector()
        {
            if (IsNumeric && Length % VectorLength == 0 && SIMD && IsNumeric)
            {
                IsVectorizable = true;
            }
            else
            {
                IsVectorizable = false;
            }
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe ref T Read(int index)
        {
            // return (T*) (_ptr + byteOffset);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(voidPtr), index);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void Write(int index, T value)
        {

            ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(voidPtr), index);
            v = value;
        }


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public bool Acquire()
        {
            if (IsNotAllocated || IsInvalid)
                return false;
            bool success = false;
            DangerousAddRef(ref success);
            if (success)
            {
                Jem.IncrementRefCount(handle);
            }
            return success;
        }

        public bool Release(int n)
        {
            bool r = false;
            for (int i = 0; i < n; i++)
            {
                r = Release();
                if (r)
                {
                    continue;
                }
                else
                {
                    return r;
                }
            }
            return r;
        }
        #endregion

        #region Values

        public bool EqualTo(T[] array)
        {
            ThrowIfNotAllocatedOrInvalid();
            return _Span.SequenceEqual<T>(new ReadOnlySpan<T>(array));

        }

        public void Fill(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            Span.Fill(value);
        }


        public void CopyFrom(T[] array)
        {
            ThrowIfNotAllocatedOrInvalid();
            Span<T> s = _Span;
            new Span<T>(array).CopyTo(_Span);
        }

        public T[] CopyToArray()
        {
            ThrowIfNotAllocatedOrInvalid();
            T[] a = new T[this.Length];
            _Span.CopyTo(new Span<T>(a));
            return a;
        }

        public unsafe Span<C> GetSpan<C>(int index = 0, int length = 1) where C : struct, IEquatable<C>
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            ulong s = (ulong) (index * ElementSizeInBytes + length * Unsafe.SizeOf<C>());
            if (s > SizeInBytes)
            {
                SizeIsOutOfRange(s);
            }
            void* p = BufferHelpers.Add<T>(handle, index).ToPointer();
            return new Span<C>(p, length);
        }

        public unsafe Span<T> Slice(int index) => GetSpan<T>(index, Length - index);

        public unsafe Span<T> Slice(int start, int end)
        {
            if (start >= end)
            {
                throw new ArgumentOutOfRangeException($"The end {end} of the slice must be greater than the start {start}.");
            }
            else
            {
                return GetSpan<T>(start, end - start);
            }
        }

        public unsafe Vector<C> GetVector<C>() where C : struct, IEquatable<C>, IConvertible, IComparable, IFormattable
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotNumeric();
            if (this.Length != Vector<C>.Count)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<C>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            else
            {
                return Unsafe.Read<Vector<C>>(voidPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Vector<T> GetSliceAsVector(int index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotNumeric();
            if ((index + VectorLength) > Length)
            {
                BufferIndexIsOutOfRange(index);
            }
            return Unsafe.Read<Vector<T>>(BufferHelpers.Add<T>(handle, index).ToPointer());
        }
        #endregion


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfNotAllocatedOrInvalid()
        {
            if (IsNotAllocated)
            {
                BufferIsNotAllocated();
            }
            else if (IsInvalid)
            {
                HandleIsInvalid();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfRetained()
        {
            if (IsRetained)
            {
                throw new InvalidOperationException($"SafeBuffer<{typeof(T)}[{Length}] has outstanding references.");
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfNotAllocated()
        {
            if (IsNotAllocated)
            {
                BufferIsNotAllocated();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfCannotAcquire()
        {
            if (!Acquire())
            {
                throw new InvalidOperationException("Could not acquire handle.");
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfNotVectorisable()
        {
            if (!IsVectorizable)
            {
                BufferIsNotVectorisable();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfNotNumeric()
        {
            if (!IsNumeric)
            {
                BufferIsNotNumeric();
            }
        }


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfIndexOutOfRange(int index)
        {
            if (index < 0 || index >= Length)
            {
                BufferIndexIsOutOfRange(index);
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void ThrowIfLengthOutOfRange(int length)
        {
            if (length < 0 || length>= Length)
            {
                LengthIsOutOfRange(length);
            }
        }


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void HandleIsInvalid()
        {
            throw new InvalidOperationException("The handle is invalid.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void BufferIsNotAllocated()
        {
            Contract.Assert(false, "Unallocated safe buffer used.");
            throw new InvalidOperationException("Unallocated safe buffer used.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void BufferIsNotVectorisable()
        {
            Contract.Assert(false, "Buffer is not vectorisable.");
            throw new InvalidOperationException("Buffer is not vectorisable.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void BufferIsNotNumeric()
        {
            Contract.Assert(false, "Buffer is not numeric.");
            throw new InvalidOperationException("Buffer is not numeric.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void BufferIndexIsOutOfRange(int index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            throw new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void LengthIsOutOfRange(int length)
        {
            Contract.Assert(false, $"Length {length} exceeds buffer length.");
            throw new Exception($"Length {length} exceeds buffer length.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static void SizeIsOutOfRange(ulong size)
        {
            Contract.Assert(false, $"Length {size} exceeds buffer size.");
            throw new Exception($"Length {size} exceeds buffer size.");
        }

        #region Arithmetic
        public void VectorFill(T value)
        {
            Span<Vector<T>> s = GetSpan<Vector<T>>();
            Vector<T> fill = new Vector<T>(value);
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = fill;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorMultiply(T value)
        {
            ThrowIfNotVectorisable();
            Span<Vector<T>> vectorSpan = GetSpan<Vector<T>>();
            Vector<T> mulVector = new Vector<T>(value);
            for (int i = 0; i < vectorSpan.Length; i++)
            {
                vectorSpan[i] = Vector.Multiply(vectorSpan[i], mulVector);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VectorSqrt()
        {
            ThrowIfNotVectorisable();
            Span<Vector<T>> vector = GetSpan<Vector<T>>();
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = Vector.SquareRoot(vector[i]);
            }
        }
        #endregion

        #endregion

        #region Operators
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ThrowIfNotAllocatedOrInvalid();
                ThrowIfIndexOutOfRange(index);
                return ref this.Read(index);
            }
         }
        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly uint ElementSizeInBytes = (uint) JemUtil.SizeOfStruct<T>();
        protected static readonly UInt64 NotAllocated = UInt64.MaxValue;
        protected static bool IsNumeric = JemUtil.IsNumericType<T>();
        protected static int VectorLength = IsNumeric ? Vector<T>.Count : 0;
        protected static bool SIMD = Vector.IsHardwareAccelerated;
        protected internal unsafe void* voidPtr;
        //Debugger Display = {T[length]}
        protected string DebuggerDisplay => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion
    }
}
