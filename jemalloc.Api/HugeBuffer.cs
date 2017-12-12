using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace jemalloc
{
    public abstract class HugeBuffer<T> : SafeHandle, IEnumerable<T> where T : struct
    {
        #region Constructors
        protected HugeBuffer(ulong length, params T[] values) : base(IntPtr.Zero, true)
        {
            ulong l = (ulong)values.Length;
            if (BufferHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }
            if ((ulong) values.Length > length)
            {
                throw new ArgumentException("The length of the list of values must be smaller or equal to the length of the buffer");
            }
            SizeInBytes = NotAllocated;
            base.SetHandle(Allocate(length));
            if (IsAllocated)
            {
                for (ulong i = 0; i < l; i++)
                {
                    this[i] = values[i];
                }
            }

        }
        #endregion

        #region Overriden members
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            return Jem.Free(handle);        
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
        #endregion

        #region Properties
        public ulong Length { get; protected set; }

        public ulong SizeInBytes { get; protected set; }

        public bool IsNotAllocated => SizeInBytes == NotAllocated;

        public bool IsAllocated => !IsNotAllocated;

        public bool IsVectorizable { get; protected set; }

        #endregion

        #region Methods
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public unsafe bool Acquire()
        {
            ThrowIfNotAllocated();
            bool result = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                DangerousAddRef(ref result);
            }
            return result;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Release()
        {
            ThrowIfNotAllocated();
            DangerousRelease();
        }

        public unsafe Span<T> Span(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            GetSegment(index, out void* ptr, out int offset);
            return new Span<T>(ptr, offset + 1);
        }

        public unsafe void Fill(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            for (int i = 0; i < segments.Length - 1; i++)
            {
                Span<T> s = new Span<T>(segments[i], Int32.MaxValue);
                s.Fill(value);
            }
            Span<T> last = Span(Length - 1);
            last.Fill(value);
        }

        public T[] CopyToArray()
        {
            ThrowIfNotAllocatedOrInvalid();
            if (this.Length > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("This length of this array exceeds the max length of a managed array.");
            }
            T[] a = new T[this.Length];
            for (ulong i = 0; i < this.Length; i++)
            {
                a[i] = this[i];
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Vector<T> ToSingleVector()
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotVectorisable();
            if (this.Length != (ulong) VectorLength)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<T>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            Span<T> span = new Span<T>(segments[0], VectorLength);
            Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            return vector[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Vector<T> SliceToVector(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotNumeric();
            if ((Length - index) < (ulong) VectorLength)
            {
                ThrowIfIndexOutOfRange(index);
            }
            T v = this[index];
            
            GetSegment(index, out void* ptr, out int offset);
            IntPtr p = new IntPtr(ptr);
            Span<T> span = new Span<T>(BufferHelpers.Add<T>(p, offset).ToPointer(), VectorLength);
            Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            return vector[0];
        }


        public unsafe void VectorMultiply(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotVectorisable();
            T[] factor = new T[VectorLength];
            for (int f = 0; f < VectorLength; f++)
            {
                factor[f] = value;
            }
            Vector<T> factorVector = new Vector<T>(factor);
            for (int h = 0; h < segments2.Length; h++)
            {
                Span<T> span = new Span<T>(segments[h], segments2[h].Item2);
                Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
                int i = 0;
                for (i = 0; i < vector.Length; i++)
                {
                    vector[i] = Vector.Multiply(vector[i], factorVector);
                }
            }
        }

        public unsafe void VectorSqrt()
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotVectorisable();
            for (int h = 0; h < segments2.Length; h++)
            {
                Span<T> span = new Span<T>(segments[h], segments2[h].Item2);
                Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Vector.SquareRoot(vector[i]);
                }
            }
        }

        public unsafe ref T DangerousGetRef(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            GetSegment(index, out void* ptr, out int offset);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(ptr), offset);
        }

        protected unsafe virtual IntPtr Allocate(ulong length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            Contract.EndContractBlock();
            ulong s = checked(length * ElementSizeInBytes);
            handle = Jem.Calloc(length, ElementSizeInBytes);
            if (handle != IntPtr.Zero)
            {
                voidPtr = handle.ToPointer();
                Length = length;
                SizeInBytes = s;
                InitSegments();
                InitVectors();
            }
            return handle;
        }

        protected unsafe void InitSegments()
        {
            int n = (int) ((Length - 1) / Int32.MaxValue) + 1;
            segments = new void*[n];
            segments2 = new Tuple<IntPtr, int>[segments.Length];
            for (int i = 0; i < n; i++)
            {
                segments[i] = (handle + i * Int32.MaxValue).ToPointer();
                if (i < n - 1)
                {
                    segments2[i] = new Tuple<IntPtr, int>(new IntPtr(segments[i]), (i + 1) * Int32.MaxValue);
                }
            }
            segments2[n - 1] = new Tuple<IntPtr, int>(new IntPtr(segments[n - 1]), (int)(Length - (ulong)((n - 1) * Int32.MaxValue)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetSegmentIndex(ulong index)
        {
            ThrowIfIndexOutOfRange(index);
            return (int)(index / Int32.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void GetSegment(ulong index, out void* ptr, out int offset)
        {
            int s = GetSegmentIndex(index);
            int l = segments.Length;
            ptr = segments[s];
            offset = (int) (index - ((ulong)(l-1) * (ulong) Int32.MaxValue));
        }

        protected unsafe void InitVectors()
        {
            if (Length % (ulong)VectorLength == 0 && SIMD)
            {
         
                IsVectorizable = true;
            }
            else
            {
                IsVectorizable = false;
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected unsafe void AcquirePointer(ref byte* pointer)
        {
            ThrowIfNotAllocated();
            pointer = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                bool result = false;
                DangerousAddRef(ref result);
                pointer = (byte*)handle;
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected T Read(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);

            // return (T*) (seg_ptr + byteOffset);
            T value = default;
            ref T ret = ref value;
            bool mustCallRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustCallRelease);
                unsafe
                {
                    GetSegment(index, out void* ptr, out int offset);
                    return Unsafe.Add(ref Unsafe.AsRef<T>(ptr), offset);
                }
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected T Write(ulong index, T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            // return (T*) (_ptr + byteOffset);
            bool mustCallRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustCallRelease);
                unsafe
                {
                    GetSegment(index, out void* ptr, out int offset);
                    ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(ptr), offset);
                    v = value;
                    return v;
                }
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
        }

        public IEnumerator<T> GetEnumerator() => new HugeBufferEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => new HugeBufferEnumerator<T>(this);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private void ThrowIfIndexOutOfRange(ulong index)
        {
            if (index >= Length)
            {
                BufferIndexIsOutOfRange(index);
            }
         }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
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
        [DebuggerStepThrough]
        private void ThrowIfNotAllocated()
        {
            if (IsNotAllocated)
            {
                BufferIsNotAllocated();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private void ThrowIfNotVectorisable()
        {
            if (!IsVectorizable)
            {
                BufferIsNotVectorisable();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private void ThrowIfNotNumeric()
        {
            if (!IsNumeric)
            {
                BufferIsNotNumeric();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private static InvalidOperationException HandleIsInvalid()
        {
            Contract.Assert(false, "The handle is invalid.");
            return new InvalidOperationException("The handle is invalid.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private static InvalidOperationException BufferIsNotAllocated()
        {
            Contract.Assert(false, "Unallocated safe buffer used.");
            return new InvalidOperationException("Unallocated safe buffer used.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private static InvalidOperationException BufferIsNotVectorisable()
        {
            Contract.Assert(false, "Buffer is not vectorisable.");
            return new InvalidOperationException("Buffer is not vectorisable.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private static InvalidOperationException BufferIsNotNumeric()
        {
            Contract.Assert(false, "Buffer is not numeric.");
            return new InvalidOperationException("Buffer is not numeric.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private static IndexOutOfRangeException BufferIndexIsOutOfRange(ulong index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            return new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        private static ConstructorInfo VectorInternalConstructorUsingPointer = typeof(Vector<T>).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
     new Type[] { typeof(void*), typeof(int) }, null);
        #endregion

        #region Operators
        public T this[ulong index]
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get => Read(index);
            
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            set => Write(index, value);
        }
        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly uint ElementSizeInBytes = (uint) JemUtil.SizeOfStruct<T>();
        protected static readonly UInt64 NotAllocated = UInt64.MaxValue;
        protected static readonly bool IsNumeric = JemUtil.IsNumericType<T>();
        protected static readonly int VectorLength = Vector<T>.Count;
        protected static bool SIMD = Vector.IsHardwareAccelerated;
        protected internal unsafe void* voidPtr;
        protected unsafe void*[] segments;
        protected unsafe Tuple<IntPtr, int>[] segments2;
        //Debugger Display = {T[length]}
        private string DebuggerDisplay => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion

    }
}
