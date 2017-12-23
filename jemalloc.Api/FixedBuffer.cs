using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace jemalloc
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FixedBuffer<T> : IDisposable, IRetainable, IEquatable<FixedBuffer<T>> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        #region Constructor
        public FixedBuffer(int length)
        {
            _Ptr = IntPtr.Zero;
            _Length = 0;
            _SizeInBytes = 0;
            _Timestamp = 0;
            RefCount = 0;
            Allocate(length);
        }
        #endregion

        #region Implemented members
        void IRetainable.Retain() => ++RefCount;
        bool IRetainable.Release() => Free();
        bool IEquatable<FixedBuffer<T>>.Equals(FixedBuffer<T> buffer)
        {
            return this._Ptr == buffer.Ptr && this.Length == buffer.Length && this._Timestamp == buffer.Timestamp;
        }
        #region Disposer
        void IDisposable.Dispose()
        {
            if (IsRetained)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) has outstanding references.");
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Disposer
        private void Dispose(bool disposing)
        {
            Free();
        }
        #endregion

        #endregion

        #region Properties
        public bool IsInvalid => _Ptr == IntPtr.Zero;

        public bool IsRetained => RefCount > 0;

        public int Length
        {
            get
            {
                ThrowIfInvalid();
                return _Length;
            }

        }

        public ulong Size
        {
            get
            {
                ThrowIfInvalid();
                return _SizeInBytes;
            }

        }

        internal IntPtr Ptr
        {
            get
            {
                ThrowIfInvalid();
                return _Ptr;
            }
        }

        internal long Timestamp
        {
            get
            {
                ThrowIfInvalid();
                return _Timestamp;
            }
        }
        public unsafe Span<T> Span
        {
            get
            {
                ThrowIfInvalid();
                return new Span<T>(_Ptr.ToPointer(), _Length);
            }
        }

        public int RefCount { get; internal set; }

        #endregion

        #region Methods
        private unsafe bool Allocate(int length)
        {
            _Ptr = Jem.CallocFixedBuffer<T>((ulong)length, ElementSizeInBytes);
            if (_Ptr != IntPtr.Zero)
            {
                _Length = length;
                _SizeInBytes = (ulong)_Length * ElementSizeInBytes;
                _Timestamp = DateTime.Now.Ticks;

                return true;
            }
            else return false;
        }

        public unsafe MemoryHandle Acquire()
        {
            ThrowIfInvalid();
            ThrowIfRefCountNonZero();
            RefCount++;
            return new MemoryHandle(this, this.Ptr.ToPointer());
        }

        public void Release()
        {
            ThrowIfInvalid();
            ThrowIfRefCountNonZero();
            RefCount--;
        }

        public bool Free()
        {
            ThrowIfInvalid();
            ThrowIfRefCountNonZero();
            if (Interlocked.Exchange(ref _Ptr, IntPtr.Zero) != IntPtr.Zero)
            {
                if (Jem.Free(_Ptr))
                {
                    _Ptr = IntPtr.Zero;
                    return true;
                }
                else
                {
                    return false;
                    //throw new Exception($"Could not free buffer at pointer {_Ptr}.");
                }
            }
            else
            {
                return false;
            }
        }


        public void Fill(T value)
        {
            ThrowIfInvalid();
            Span.Fill(value);
        }

        private void ThrowIfInvalid()
        {
            if (IsInvalid)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) is invalid.");
            }
        }

        private void ThrowIfRefCountNonZero()
        {
            if (RefCount > 0)
            {
                throw new InvalidOperationException($"FixedBuffer<{typeof(T).Name}>({this._Length}) has RefCount {RefCount}.");
            }
        }


        #endregion

        #region Operators
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (IsInvalid)
                    throw new InvalidOperationException("The buffer is invalid.");
                if (index >= (Length))
                    throw new IndexOutOfRangeException();
                unsafe
                {
                    return ref Unsafe.Add(ref Unsafe.AsRef<T>(Ptr.ToPointer()), index);
                }
            }
        }
        #endregion

        #region Fields
        private static readonly Type ElementType = typeof(T);
        private static readonly ulong ElementSizeInBytes = (ulong) JemUtil.SizeOfStruct<T>();
        private IntPtr _Ptr;
        private ulong _SizeInBytes;
        private int _Length;
        private long _Timestamp;
        #endregion
    }
}
