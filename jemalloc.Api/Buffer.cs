using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Linq;
using System.Text;

namespace jemalloc
{
    #region Enums
    public enum AllocationType
    {
        HEAP = 0,
        STACK = 1
    }
    #endregion

    public abstract class Buffer<T> : SafeHandle where T : struct
    {
        #region Constructors
        protected Buffer(uint length) : base(IntPtr.Zero, true)
        {
            /*
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }*/
            base.SetHandle(Allocate());
        }
        #endregion

        #region Overriden members
        protected override bool ReleaseHandle()
        {
            Je.Free(handle);
            handle = IntPtr.Zero;
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
        #endregion

        #region Properties
        public uint Length { get; protected set; }
        public ulong SizeInBytes
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return sizeInBytes;
            }
        }
        public AllocationType AllocationType { get; protected set; }
        #endregion

        #region Methods
        protected virtual IntPtr Allocate()
        {
            handle = Je.Calloc(Length, (ulong) ElementSizeInBytes);
            sizeInBytes = Length * ElementSizeInBytes;
            return handle;
        }

        public unsafe Span<T> Span()
        {
            Contract.Requires(handle != IntPtr.Zero);
            return new Span<T>((void*)handle, (int) Length);
        }
        
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public unsafe void AcquirePointer(ref byte* pointer)
        {
            Contract.Requires(handle != IntPtr.Zero);
            pointer = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                bool junk = false;
                DangerousAddRef(ref junk);
                pointer = (byte*)handle;
            }
        }
        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly uint ElementSizeInBytes = (uint) Marshal.SizeOf<T>();
        protected Func<IntPtr> StackAllocate;
        private static readonly Type Int16CLRType = typeof(Int16);
        private static readonly Type UInt16CLRType = typeof(UInt16);
        private static readonly Type Int32CLRType = typeof(Int32);
        private static readonly Type UInt32CLRType = typeof(UInt32);
        private static readonly Type Int64CLRType = typeof(Int64);
        private static readonly Type UInt64CLRType = typeof(UInt64);
        private static readonly Type SingleCLRType = typeof(Single);
        private static readonly Type DoubleCLRType = typeof(Double);
        private static readonly Type StringCLRType = typeof(String);
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>(new Type[] 
        {
            Int16CLRType, UInt16CLRType, Int32CLRType, UInt32CLRType, Int64CLRType, UInt64CLRType,
            SingleCLRType, DoubleCLRType
        });
        private ulong sizeInBytes = 0;
        #endregion

        #region Operators

        #endregion

        
    }
}
