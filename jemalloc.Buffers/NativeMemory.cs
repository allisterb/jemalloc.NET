using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace jemalloc.Buffers
{
    public class NativeMemory<T> : OwnedMemory<T> where T : struct
    {
        #region Constructors
        public NativeMemory(int length) : base()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }
            Allocate();
            this.length = length;
        }

        public NativeMemory(NativeMemory<T> buffer) : base()
        {
            buffer.Retain();
            this.ptr = buffer.ptr;
            this.length = buffer.length;
        }
        #endregion

        #region Overriden members
        public override int Length => length;
        
        protected override bool IsRetained => referenceCount > 0;

        public override bool IsDisposed => disposed;

        public unsafe override Span<T> Span
        {
            get
            {
                return new Span<T>(ptr.ToPointer(), Length);
            }
        }

        public override void Retain()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("This buffer has been disposed.");
            }
            else
            {
                Interlocked.Increment(ref referenceCount);
            }
        }

        public override bool Release()
        {
            int newRefCount = Interlocked.Decrement(ref referenceCount);
            if (newRefCount < 0)
            {
                throw new InvalidOperationException("Reference count was decremented to < 0.");
            }
            else if (newRefCount == 0)
            {
                OnNoReferences();
                return false;
            }
            else
            {
                return true;
            }
        }

        public override MemoryHandle Pin()
        {
            Retain();
            unsafe
            {
                return new MemoryHandle(this, ptr.ToPointer());
            }
        }

        protected override bool TryGetArray(out ArraySegment<T> arraySegment)
        {
            arraySegment = default;
            return false;
        }
        #endregion

        #region Properties
        public ulong SizeInBytes => sizeInBytes;
        public int ReferenceCount => referenceCount;
        #endregion

        #region Methods
        public Tensor<T> ToTensor(params int[] dimensions)
        {
            Retain();
            return new DenseTensor<T>(this.Memory, new ReadOnlySpan<int>(dimensions), false);
        }

        public NativeMemoryVectors<T> AsVectors()
        {
            return new NativeMemoryVectors<T>(this);
        }

        protected virtual IntPtr Allocate()
        {
            ptr = Je.Calloc((ulong) length, (ulong)ElementSizeInBytes);
            sizeInBytes = (ulong) Length * (ulong) ElementSizeInBytes;
            return ptr;
        }

        protected virtual void OnNoReferences() {}
        #endregion


        #region Disposer and finalizer
        protected override void Dispose(bool disposing)
        {
            if (ptr != null)
            {
                Je.Free(ptr);
            }
            ptr = IntPtr.Zero;
            disposed = disposing;
        }

        ~NativeMemory()
        {
            Dispose(false);
        }



        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly int ElementSizeInBytes = Marshal.SizeOf<T>();
        IntPtr ptr;
        int length;
        ulong sizeInBytes;
        int referenceCount;
        bool disposed;
        #endregion

    }
}
