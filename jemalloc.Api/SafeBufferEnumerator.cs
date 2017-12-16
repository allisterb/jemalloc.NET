using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace jemalloc
{
    /// <summary>Enumerates the elements of a <see cref="SafeBuffer{T}"/>.</summary>
    public class SafeBufferEnumerator<T> : IEnumerator, IEnumerator<T> where T : struct, IEquatable<T>
    {
        /// <summary>The SafeBuffer being enumerated.</summary>
        private readonly SafeBuffer<T> _buffer;
        /// <summary>The next index to yield.</summary>
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SafeBufferEnumerator(SafeBuffer<T> buffer)
        {
            _buffer = buffer;
            _buffer.Acquire();
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = _index + 1;
            if (index < _buffer.Length)
            {
                _index = index;
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public void Reset()
        {
            _index = -1;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer[_index];
        }

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer[_index];
        }

        void IDisposable.Dispose()
        {
            _buffer.Release();
        }
    }

}
