using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;
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
            this.length = length;
            Allocate();
        }

        public NativeMemory(params T[] values) : this(values.Length)
        {
            Span<T> span = Span;
            for (int i = 0; i < length; i++)
            {
                span[i] = values[i];
            }
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
            if (IsDisposed)
            {
                throw new InvalidOperationException("This buffer has been disposed.");
            }
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
        public Tensor<T> AsTensor(params int[] dimensions)
        {
            Retain();
            return new DenseTensor<T>(this.Memory, new ReadOnlySpan<int>(dimensions), false);
        }

        public unsafe Vector<T> AsVector()
        {
            if (length < Vector<T>.Count)
            {
                throw new InvalidOperationException($"The length of the memory buffer must be at least {Vector<T>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            Retain();
            object[] args = new object[2] { ptr, 0 };
            Vector<T> v = (Vector<T>)VectorInternalConstructorUsingPointer.Invoke(args);
            return v;
        }

        protected virtual IntPtr Allocate()
        {
            ptr = Je.Calloc((ulong)length, (ulong) ElementSizeInBytes);
            sizeInBytes = (ulong)Length * (ulong) ElementSizeInBytes;
            return ptr;
        }

        protected virtual void OnNoReferences() {}
        
        #endregion

        #region Fields
        protected static readonly Type CLRType = typeof(T);
        protected static readonly T Element = default;
        protected static readonly int ElementSizeInBytes = Marshal.SizeOf(Element);
        private static ConstructorInfo VectorInternalConstructorUsingPointer = typeof(Vector<T>).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
            new Type[] { typeof(void*), typeof(int) }, null);
        IntPtr ptr;

        int length;
        ulong sizeInBytes;
        int referenceCount;
        bool disposed;

        #endregion

        #region Disposer and finalizer
        protected override void Dispose(bool disposing)
        {
            if (disposing && referenceCount > 0)
            {
                throw new InvalidOperationException($"This buffer cannot be disposed until all references are released. Reference count is {referenceCount}.");
            }
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
    }
}
