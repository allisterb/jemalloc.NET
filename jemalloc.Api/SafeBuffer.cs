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
using System.Linq;
using System.Text;

namespace jemalloc
{
    public abstract class SafeBuffer<T> : SafeHandle, IEnumerable<T> where T : struct
    {
        #region Constructors
        protected SafeBuffer(int length, params T[] values) : base(IntPtr.Zero, true)
        {
            if (BufferHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }
            if (values.Length > length)
            {
                throw new ArgumentException("The length of the list of values must be smaller or equal to the length of the buffer");
            }
            SizeInBytes = NotAllocated;
            base.SetHandle(Allocate(length));
            if (!IsNotAllocated)
            {
                for (int i = 0; i < values.Length; i++)
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
        public int Length { get; protected set; }

        public ulong SizeInBytes { get; protected set; }

        public bool IsNotAllocated => SizeInBytes == NotAllocated;
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

        public unsafe Span<T> Span()
        {
            ThrowIfNotAllocatedOrInvalid();
            return new Span<T>((void*)handle, (int)Length);
        }

        public void Fill(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            Span<T> s = Span();
            s.Fill(value);
        }

        public unsafe ref T DangerousAsRef(int index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(voidPtr), index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T[] UncheckedCopyToArray()
        {
            T[] a = new T[this.Length];
            for(int i = 0; i < this.Length; i++)
            {
                a[i] = this[i];
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T[] UncheckedCopyToArray(int index, int length)
        {
            T[] a = new T[length];
            for (int i = 0; i < length; i++)
            {
                a[i] = this[index + i];
            }
            return a;
        }

        public Vector<T> CopyToVector()
        {
            if (!JemUtil.IsNumericType<T>())
            {
                throw new Exception("Only numeric types can be read as vectors.");
            }
            else if (this.Length != Vector<T>.Count)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<T>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            T[] values = this.UncheckedCopyToArray();
            return new Vector<T>(values);
        }

        public unsafe Vector<T> UncheckedCopyToVector(int index)
        {
            return new Vector<T>(this.UncheckedCopyToArray(index, VectorLength));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector<T> ToVector()
        {
            if (!JemUtil.IsNumericType<T>())
            {
                throw new Exception("Only numeric types can be read as vectors.");
            }
            else if (this.Length != Vector<T>.Count)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<T>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            Span<T> span = Span();
            Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            return vector[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector<T> ToVector(int index)
        {
            Span<T> span = Span().Slice(index, VectorLength);
            return span.NonPortableCast<T, Vector<T>>()[0];
        }


        public void VectorMultiply(T value)
        {
            if (!Vector.IsHardwareAccelerated)
            {
                throw new Exception();
            }
            if (this.Length % VectorLength != 0)
            {
                throw new ArithmeticException($"The length of the buffer {Length} is not a multiple of the VectorLength {VectorLength}.");
            }
            T[] fill = new T[VectorLength];
            for (int f = 0; f < VectorLength; f++)
            {
                fill[f] = value;
            }
            Vector<T> fillVector = new Vector<T>(fill);
            Span <T> span = Span();
            Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            int i = 0;
            for (i = 0; i < vector.Length; i ++)
            {
                Vector<T> v = vector[i];
                vector[i] = Vector.Multiply(v, fillVector);
            }
        }

        public void VectorSqrt()
        {
            if (!Vector.IsHardwareAccelerated)
            {
                throw new Exception();
            }
            if (this.Length % VectorLength != 0)
            {
                throw new ArithmeticException($"The length of the buffer {Length} is not a multiple of the VectorLength {VectorLength}.");
            }
            Span<T> span = Span();
            Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            int i = 0;
            for (i = 0; i < vector.Length; i++)
            {
                Vector<T> v = vector[i];
                vector[i] = Vector.SquareRoot(v);
            }
        }

        protected unsafe virtual IntPtr Allocate(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");
            Contract.EndContractBlock();
            ulong s = checked((uint)length * ElementSizeInBytes);
            handle = Jem.Calloc((uint) length, ElementSizeInBytes);
            if (handle != IntPtr.Zero)
            {
                voidPtr = handle.ToPointer();
                Length = length;
                SizeInBytes = s;
            }
            return handle;
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
        protected T Read(int index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);

            // return (T*) (_ptr + byteOffset);
            T value = default;
            ref T ret = ref value;
            bool mustCallRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustCallRelease);
                unsafe
                {
                    return Unsafe.Add(ref Unsafe.AsRef<T>(voidPtr), index);
                }
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected T Write(int index, T value)
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
                    ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(voidPtr), index);
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

        public IEnumerator<T> GetEnumerator() => new SafeBufferEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => new SafeBufferEnumerator<T>(this);

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
        private void ThrowIfIndexOutOfRange(int index)
        {
            if (index < 0 || index >= Length)
            {
                IndexIsOutOfRange(index);
            }
        }


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DebuggerStepThrough]
        private static InvalidOperationException HandleIsInvalid()
        {
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
        private static IndexOutOfRangeException IndexIsOutOfRange(int index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            return new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        private static ConstructorInfo VectorInternalConstructorUsingPointer = typeof(Vector<T>).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
            new Type[] { typeof(void*), typeof(int) }, null);

        #endregion

        #region Operators
        public unsafe T this[int index]
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get => this.Read(index);

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            set => this.Write(index, value);
         }
        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly uint ElementSizeInBytes = (uint) JemUtil.SizeOfStruct<T>();
        protected static readonly UInt64 NotAllocated = UInt64.MaxValue;
        protected static bool IsNumeric = JemUtil.IsNumericType<T>();
        protected static int VectorLength = Vector<T>.Count;
        protected internal unsafe void* voidPtr;
        //Debugger Display = {T[length]}
        protected string DebuggerDisplay => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion
    }
}
