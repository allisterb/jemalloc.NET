using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace jemalloc
{
    public ref struct Buffer<T> where T : struct
    {
        public Buffer(int length)
        {
            _Ptr = IntPtr.Zero;
            _Length = 0;
            _SizeInBytes = 0;
            _Timestamp = 0;
            _Span = new Span<T>();
            unsafe
            {
                _VoidPointer = (void*)_Ptr;
            }
            Allocate(length);
        }
        
        #region Properties
        public bool IsInvalid
        {
            get
            {
                return _Ptr == IntPtr.Zero;
            }
        }
        public int Length
        {
            get
            {
                if (IsInvalid)
                {
                    return 0;
                }
                else
                {
                    return _Length;
                }
            }
            
        }

        public ulong Size
        {
            get
            {
                if (IsInvalid)
                {
                    return 0;
                }
                else
                {
                    return _SizeInBytes;
                }
            }

        }

        public IntPtr Ptr
        {
            get
            {
                return _Ptr;
                
            }

        }

        public Span<T> Span
        {
            get
            {
                if (IsInvalid)
                {
                    throw new InvalidOperationException("The buffer is invalid.");
                }
                else
                {
                    return _Span;
                }
            }

        }

        #endregion

        #region Methods
        private unsafe bool Allocate(int length)
        {
            _Ptr = Jem.Calloc((ulong) length, ElementSizeInBytes);
            if (_Ptr != IntPtr.Zero)
            {
                _Length = length;
                _SizeInBytes = (ulong)_Length * ElementSizeInBytes;
                _Timestamp = DateTime.Now.Ticks;
                _VoidPointer = _Ptr.ToPointer();
                _Span = new Span<T>(_VoidPointer, _Length);
                return true;
            }
            else return false;
        }

        public void Release()
        {
            if (IsInvalid)
            {
                return;
            }
            else if (Interlocked.Exchange(ref _Ptr, IntPtr.Zero) != IntPtr.Zero)
            {
                Jem.Free(_Ptr);
            }
        }

       
        public void Fill(T value)
        {
            if (IsInvalid)
                throw new InvalidOperationException("The buffer is invalid.");
            Span.Fill(value);
        }
        #endregion

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
                    return ref Unsafe.Add(ref Unsafe.AsRef<T>(_VoidPointer), index);
                }
            }
        }

        #region Fields
        private static readonly Type ElementType = typeof(T);
        private static readonly ulong ElementSizeInBytes = (ulong) JemUtil.SizeOfStruct<T>();
        private ulong _SizeInBytes;
        private int _Length;
        private IntPtr _Ptr;
        private unsafe void* _VoidPointer;
        private long _Timestamp;
        private Span<T> _Span;
        #endregion
    }
}
