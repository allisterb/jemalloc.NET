using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace jemalloc
{
    public abstract class HugeBuffer<T> : SafeHandle, IEnumerable<T> where T : struct
    {
        #region Constructors
        protected HugeBuffer(ulong length) : base(IntPtr.Zero, true)
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
        public ulong Length { get; protected set; }

        public ulong SizeInBytes { get; protected set; }

        public unsafe void* VoidPtr { get; protected set; }

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

        public unsafe Span<T> Span(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            GetSegment(index, out void* ptr, out int offset);
            return new Span<T>(ptr, offset);
        }

        public unsafe void Fill(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            for (int i = 0; i < segments.Length - 1; i++)
            {
                Span<T> s = new Span<T>(segments[i], Int32.MaxValue);
                s.Fill(value);
            }
            Span<T> last = Span(Length);
            last.Fill(value);
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
                VoidPtr = handle.ToPointer();
                Length = length;
                SizeInBytes = s;
                InitSegments();
            }
            return handle;
        }


        protected unsafe void InitSegments()
        {
            int n = (int) (Length / Int32.MaxValue) + 1;
            segments = new void*[n];
            for (int i = 0; i < n; i++)
            {
                segments[i] = (handle + i * Int32.MaxValue).ToPointer();
            }
            
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
            ptr = segments[GetSegmentIndex(index)];
            offset = (int) (index - (ulong)(GetSegmentIndex(index) * Int32.MaxValue));
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
        private void ThrowIfIndexOutOfRange(ulong index)
        {
            if (index >= Length)
            {
                IndexIsOutOfRange(index);
            }
         }

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
        private static InvalidOperationException HandleIsInvalid()
        {
            Contract.Assert(false, "The handle is invalid.");
            return new InvalidOperationException("The handle is invalid.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static InvalidOperationException BufferIsNotAllocated()
        {
            Contract.Assert(false, "Unallocated safe buffer used.");
            return new InvalidOperationException("Unallocated safe buffer used.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static IndexOutOfRangeException IndexIsOutOfRange(ulong index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            return new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        #endregion

        #region Operators
        public T this[ulong index]
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get
            {
                return Read(index);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
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
        protected unsafe void*[] segments;
        //Debugger Display = {T[length]}
        private string DebuggerDisplay => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion

    }
}
