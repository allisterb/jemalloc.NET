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
    public class NativeMemoryVectors<T> : OwnedMemory<Vector<T>> where T : struct
    {
        #region Constructors
        public NativeMemoryVectors(NativeMemory<T> buffer) : base()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }
            memory = buffer;
        }
        #endregion

        #region Overriden members
        public override int Length => memory.Length;

        protected override bool IsRetained => referenceCount > 0;

        public override bool IsDisposed => memory.IsDisposed;

        public unsafe override Span<Vector<T>> Span
        {
            get
            {
                return memory.Span.NonPortableCast<T, Vector<T>>();
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
                memory.Retain();
                Interlocked.Increment(ref referenceCount);
            }
        }

        public override bool Release()
        {
            memory.Release();
            Interlocked.Decrement(ref referenceCount);
            if (memory.ReferenceCount == 0)
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

        protected override bool TryGetArray(out ArraySegment<Vector<T>> arraySegment)
        {
            arraySegment = default;
            return false;
        }
        #endregion

        #region Properties
        public ulong SizeInBytes => memory.SizeInBytes;
        #endregion

        #region Methods
        protected virtual void OnNoReferences() { }
        #endregion


        #region Disposer and finalizer
        protected override void Dispose(bool disposing)
        {
            disposed = disposing;
        }

        ~NativeMemoryVectors()
        {
            Dispose(false);
        }



        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly int ElementSizeInBytes = Marshal.SizeOf<T>();
        NativeMemory<T> memory;
        IntPtr ptr;
        int length;
        ulong sizeInBytes;
        int referenceCount;
        bool disposed;
        #endregion

    }
}
