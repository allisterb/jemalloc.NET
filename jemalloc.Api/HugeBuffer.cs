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
        public bool Acquire()
        {
            if (IsNotAllocated || IsInvalid)
                return false;
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
            if (IsNotAllocated || IsInvalid)
                return;
            DangerousRelease();
        }

        public unsafe Span<T> AcquireSpan(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            ThrowIfCannotAcquire();
            GetSegment(index, out void* ptr, out int offset);
            return new Span<T>(ptr, offset + 1);
        }

        public unsafe void Fill(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfCannotAcquire();
            for (int i = 0; i < segments.Length - 1; i++)
            {                
                Span<T> s = new Span<T>(segments[i].ToPointer(), Int32.MaxValue);
                s.Fill(value);
            }
            Span<T> last = AcquireSpan(Length - 1);
            last.Fill(value);
            Release();
        }

        public T[] CopyToArray()
        {
            ThrowIfNotAllocatedOrInvalid();
            if (this.Length > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("This length of this array exceeds the max length of a managed array.");
            }
            T[] a = new T[this.Length];
            ThrowIfCannotAcquire();
            for (ulong i = 0; i < this.Length; i++)
            {
                a[i] = this[i];
            }
            Release();
            return a;
        }

        public unsafe Vector<T> AcquireAsSingleVector()
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotVectorisable();
            if (this.Length != (ulong) VectorLength)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<T>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            ThrowIfCannotAcquire();
            Span<T> span = new Span<T>(segments[0].ToPointer(), VectorLength);
            Span<Vector<T>> vector = span.NonPortableCast<T, Vector<T>>();
            return vector[0];
        }

        public unsafe Vector<T> AcquireSliceToVector(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotNumeric();
            if ((Length - index) < (ulong) VectorLength)
            {
                ThrowIfIndexOutOfRange(index);
            }
            ThrowIfCannotAcquire();
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
            ThrowIfCannotAcquire();
            for (int h = 0; h < segments2.Length; h++)
            {
                Span<T> span = new Span<T>(segments[h].ToPointer(), segments2[h].Item2);
                Span<Vector<T>> vectorSpan = span.NonPortableCast<T, Vector<T>>();           
                T[] fill = new T[VectorLength];
                Span<T> sFil = new Span<T>(fill);
                sFil.Fill(value);
                Vector<T> fillVector = sFil.NonPortableCast<T, Vector<T>>()[0];
                vectorSpan[0] = Vector.Multiply(vectorSpan[0], fillVector);
            }
            Release();
        }

        public unsafe void VectorSqrt()
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotVectorisable();
            ThrowIfCannotAcquire();
            for (int h = 0; h < segments2.Length; h++)
            {
                Span<T> span = new Span<T>(segments[h].ToPointer(), segments2[h].Item2);
                Span<Vector<T>> vectorvectorSpan = span.NonPortableCast<T, Vector<T>>();
                for (int i = 0; i < vectorvectorSpan.Length; i++)
                {
                    vectorvectorSpan[i] = Vector.SquareRoot(vectorvectorSpan[i]);
                }
            }
            Release();
        }

        protected unsafe ref T DangerousGetRef(ulong index)
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
            segments = new IntPtr[n];
            segments2 = new Tuple<IntPtr, int>[segments.Length];
            segments[0] = handle;
            segments2[0] = new Tuple<IntPtr, int>(handle, n == 1 ? (int)Length : Int32.MaxValue);
            for (int i = 1; i < n; i++)
            {
                segments[i] = BufferHelpers.Add<T>(segments[i - 1], Int32.MaxValue);
                if (i < n - 1)
                {
                    segments2[i] = new Tuple<IntPtr, int>(segments[i], Int32.MaxValue);
                }
            }
            segments2[n - 1] = new Tuple<IntPtr, int>(segments[n - 1], (int)(Length - (ulong)((n - 1) * Int32.MaxValue)));
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
            ptr = segments[s].ToPointer();
            offset = (int) (index - ((ulong)(s) * (ulong) Int32.MaxValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void InitVectors()
        {
            if (IsNumeric && Length % (ulong)VectorLength == 0 && SIMD)
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
            pointer = null;
            if (IsNotAllocated || IsInvalid) return;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe T Read(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            // return (T*) (seg_ptr + byteOffset);
            T ret;
            ThrowIfCannotAcquire();                
            GetSegment(index, out void* ptr, out int offset);
            ret = Unsafe.Add(ref Unsafe.AsRef<T>(ptr), offset);
            Release();
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe T Write(ulong index, T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            // return (T*) (seg_ptr + byteOffset);
            ThrowIfCannotAcquire();
            GetSegment(index, out void* ptr, out int offset);
            ref T v = ref Unsafe.AsRef<T>(BufferHelpers.Add<T>(new IntPtr(ptr), offset).ToPointer());
            v = value;
            Release();
            return value; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new HugeBufferEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => new HugeBufferEnumerator<T>(this);

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
        private void ThrowIfIndexOutOfRange(ulong index)
        {
            if (index >= Length)
            {
                BufferIndexIsOutOfRange(index);
            }
         }

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
        private static InvalidOperationException HandleIsInvalid()
        {
            Contract.Assert(false, "The handle is invalid.");
            return new InvalidOperationException("The handle is invalid.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static InvalidOperationException BufferIsNotAllocated()
        {
            Contract.Assert(false, "Unallocated safe buffer used.");
            return new InvalidOperationException("Unallocated safe buffer used.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static InvalidOperationException BufferIsNotVectorisable()
        {
            Contract.Assert(false, "Buffer is not vectorisable.");
            return new InvalidOperationException("Buffer is not vectorisable.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static InvalidOperationException BufferIsNotNumeric()
        {
            Contract.Assert(false, "Buffer is not numeric.");
            return new InvalidOperationException("Buffer is not numeric.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private static IndexOutOfRangeException BufferIndexIsOutOfRange(ulong index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            return new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        #endregion

        #region Operators
        public T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Read(index);
            
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
        protected unsafe IntPtr[] segments;
        protected unsafe Tuple<IntPtr, int>[] segments2;
        //Debugger Display = {T[length]}
        private string DebuggerDisplay => string.Format("{{{0}[{1}]}}", typeof(T).Name, Length);
        #endregion

    }
}
