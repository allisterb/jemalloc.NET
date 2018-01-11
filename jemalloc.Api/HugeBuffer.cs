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
using System.Threading;

namespace jemalloc
{
    public abstract class HugeBuffer<T> : SafeHandle, IRetainable, IDisposable, IEquatable<HugeBuffer<T>>, IEnumerable<T> where T : struct, IEquatable<T>
    {
        #region Constructors
        protected HugeBuffer(ulong length, params T[] values) : base(IntPtr.Zero, true)
        {
            ulong l = (ulong)values.Length;
            if (BufferHelpers.IsReferenceOrContainsReferences<T>())
            {
                throw new ArgumentException("Only structures without reference fields can be used with this class.");
            }
            if (l > length)
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
                    voidPtr = (void*)0;
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

        public bool Equals(HugeBuffer<T> other)
        {
            ThrowIfNotAllocatedOrInvalid();
            return this.handle == other.handle && this.Length == other.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new HugeBufferEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => new HugeBufferEnumerator<T>(this);

        #endregion

        #region Properties
        public ulong Length { get; protected set; }

        public ulong SizeInBytes { get; protected set; }

        public bool IsNotAllocated => SizeInBytes == NotAllocated;

        public bool IsAllocated => !IsNotAllocated;

        public bool IsValid => !IsInvalid;

        public bool IsRetained => _RefCount > 0;

        public bool IsVectorizable { get; protected set; }

        protected int _RefCount => Jem.GetRefCount(handle);
        #endregion

        #region Methods

        #region Memory management
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
            int n = (int)((Length - 1) / Int32.MaxValue) + 1;
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int _GetSegmentIndex(ulong index)
        {
            return (int)(index / Int32.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void _GetSegment(ulong index, out void* ptr, out int offset)
        {
            int s = _GetSegmentIndex(index);
            int l = segments.Length;
            ptr = segments[s].ToPointer();
            offset = (int)(index - ((ulong)(s) * Int32.MaxValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe Span<T> _GetSegmentSpan(ulong index)
        {
            int i = _GetSegmentIndex(index);
            return new Span<T>(segments2[i].Item1.ToPointer(), segments2[i].Item2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe Span<Vector<T>> _GetSegmentVectorSpan(ulong index)
        {
            int i = _GetSegmentIndex(index);
            return new Span<Vector<T>>(segments2[i].Item1.ToPointer(), segments2[i].Item2 / VectorLength + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe ref T _Read(ulong index)
        {

            // return (T*) (seg_ptr + byteOffset);

            _GetSegment(index, out void* ptr, out int offset);
            ref T ret = ref Unsafe.Add(ref Unsafe.AsRef<T>(ptr), offset);

            return ref ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe T _Write(ulong index, T value)
        {

            // return (T*) (seg_ptr + byteOffset);
            _GetSegment(index, out void* ptr, out int offset);
            ref T v = ref Unsafe.AsRef<T>(BufferHelpers.Add<T>(new IntPtr(ptr), offset).ToPointer());
            v = value;
            return value;
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
            if (this.Length != (ulong)array.Length)
            {
                return false;
            }
            return _GetSegmentSpan(0).SequenceEqual(new ReadOnlySpan<T>(array));
        }

        public unsafe void Fill(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            for (int i = 0; i < segments.Length; i++)
            {
                Span<T> s = new Span<T>(segments2[i].Item1.ToPointer(), segments2[i].Item2);
                s.Fill(value);
            }
        }

        public void CopyFrom(T[] array)
        {
            ThrowIfNotAllocatedOrInvalid();
            new Span<T>(array).CopyTo(_GetSegmentSpan(0));
        }

        public T[] CopyToArray()
        {
            ThrowIfNotAllocatedOrInvalid();
            if (this.Length > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("This length of this array exceeds the max length of a managed array.");
            }
            T[] a = new T[this.Length];
            _GetSegmentSpan(0).CopyTo(new Span<T>(a));
            return a;
        }

        public unsafe Span<C> GetSpan<C>(ulong index = 0, int length = 1) where C : struct, IEquatable<C>
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            ulong s = (index * (ulong) ElementSizeInBytes + (ulong) length * (ulong) Unsafe.SizeOf<C>());
            if (s > SizeInBytes)
            {
                BufferSizeIsOutOfRange(s);
            }
            _GetSegment(index, out void* ptr, out int offset);
            void* p = BufferHelpers.Add<T>(new IntPtr(ptr), offset).ToPointer();
            return new Span<C>(p, length);
        }

        public unsafe Span<T> Slice(ulong start, ulong end)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(start);
            ThrowIfIndexOutOfRange(end);
            if (start >= end)
            {
                throw new ArgumentOutOfRangeException($"The end {end} of the slice must be greater than the start {start}.");
            }
            else if (end - start > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException($"The size of the slice must be less than or equal to {Int32.MaxValue}.");
            }
            _GetSegment(end, out void* ptr, out int offset);
            void* p = BufferHelpers.Add<T>(new IntPtr(ptr), offset).ToPointer();
            return new Span<T>(p, (int) (end - start));
        }


        public unsafe Vector<T> GetAsSingleVector()
        {
            ThrowIfNotAllocatedOrInvalid();
            if (this.Length != (ulong) VectorLength)
            {
                throw new InvalidOperationException($"The length of the array must be {Vector<T>.Count} elements to create a vector of type {CLRType.Name}.");
            }
            return Unsafe.Read<Vector<T>>(handle.ToPointer());
        }

        public unsafe Span<Vector<T>> GetSliceSegmentAsVectorSpan(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            if ((Length - index) < (ulong) VectorLength)
            {
                ThrowIfIndexOutOfRange(index);
            }
            T v = this[index];
            int i = _GetSegmentIndex(index);
            if (segments2[i].Item2 % VectorLength != 0)
            {
                BufferIsNotVectorisable();
            }
            return new Span<Vector<T>>(segments2[i].Item1.ToPointer(), segments2[i].Item2 / VectorLength + 1);
        }

        public unsafe Vector<T> GetSliceAsSingleVector(ulong index)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfIndexOutOfRange(index);
            if ((Length - index) < (ulong) VectorLength)
            {
                BufferIsNotVectorisable();
            }
            int i = _GetSegmentIndex(index);
            _GetSegment(index, out void* ptr, out int offset);
            IntPtr start = BufferHelpers.Add<T>(segments2[i].Item1, offset);
            return Unsafe.Read<Vector<T>>(start.ToPointer());
        }
        #endregion

        #region Vector
        /*
        public unsafe void VectorMultiply(T value)
        {
            ThrowIfNotAllocatedOrInvalid();
            ThrowIfNotNumeric();
            Vector<T> mul = new Vector<T>(value);
            for (int h = 0; h < segments2.Length; h++)
            {
                int i = 0;
                for (; i < segments2[h].Item2 - VectorLength; i++ )
                {
                    Vector<T> v = Unsafe.Read<Vector<T>>(BufferHelpers.Add<T>(segments2[h].Item1, i).ToPointer());
                    v = v * mul;
                }
                f
            }
        }*/
        #endregion

        #region Helpers
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
        private void BufferIndexIsOutOfRange(ulong index)
        {
            Contract.Assert(false, $"Index {index} into buffer is out of range.");
            throw new IndexOutOfRangeException($"Index {index} into buffer is out of range.");
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        private void BufferSizeIsOutOfRange(ulong index)
        {
            Contract.Assert(false, $"Size {index} exceeds the size of the buffer.");
            throw new IndexOutOfRangeException($"Size {index} exceeds the size of the buffer.");
        }
        #endregion

        #endregion

        #region Operators
        public ref T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _Read(index);
            
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
