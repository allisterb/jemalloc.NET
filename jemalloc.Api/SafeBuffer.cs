using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        protected SafeBuffer(int length) : base(IntPtr.Zero, true)
        {
            if (BufferHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }
            SizeInBytes = NotAllocated;
            base.SetHandle(Allocate(length));
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

        public unsafe void* VoidPtr { get; protected set; }

        public bool IsNotAllocated => SizeInBytes == NotAllocated;

        public bool IsSlice { get; protected set; }
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

        public unsafe ref T DangerousGetRef(int index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(VoidPtr), index);
        }

        protected unsafe virtual IntPtr Allocate(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            Contract.EndContractBlock();
            ulong s = checked((uint)length * ElementSizeInBytes);
            handle = Jem.Calloc((uint) length, ElementSizeInBytes);
            if (handle != IntPtr.Zero)
            {
                VoidPtr = handle.ToPointer();
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
                    return Unsafe.Add(ref Unsafe.AsRef<T>(VoidPtr), index);
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
                    ref T v = ref Unsafe.Add(ref Unsafe.AsRef<T>(VoidPtr), index);
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
        private void ThrowIfNotAllocated()
        {
            if (IsNotAllocated)
            {
                BufferIsNotAllocated();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private void ThrowIfIndexOutOfRange(int index)
        {
            if (index < 0 || index >= Length)
            {
                IndexIsOutOfRange(index);
            }
        }


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static InvalidOperationException HandleIsInvalid()
        {
            return new InvalidOperationException("The handle is invalid.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static InvalidOperationException BufferIsNotAllocated()
        {
            Contract.Assert(false, "Unallocated safe buffer used.");
            return new InvalidOperationException("Unallocated safe buffer used.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static IndexOutOfRangeException IndexIsOutOfRange(int index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            return new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        #endregion

        #region Operators
        public unsafe T this[int index]
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
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
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly uint ElementSizeInBytes = (uint) JemUtil.SizeOfStruct<T>();
        protected static readonly UInt64 NotAllocated = UInt64.MaxValue;
        //Debugger Display = {T[length]}
        private string DebuggerDisplay => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion
    }
}
