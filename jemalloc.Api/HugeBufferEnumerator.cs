using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace jemalloc
{
    /// <summary>Enumerates the elements of a <see cref="HugeBuffer{T}"/>.</summary>
    public class HugeBufferEnumerator<T> : IEnumerator, IEnumerator<T> where T : struct, IEquatable<T>
    {
        /// <summary>The span being enumerated.</summary>
        private readonly HugeBuffer<T> _buffer;
        /// <summary>The next index to yield.</summary>
        private ulong _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HugeBufferEnumerator(HugeBuffer<T> buffer)
        {
            _buffer = buffer;
            _buffer.Acquire();
            _index = UInt64.MaxValue;
        }

        /// <summary>Advances the enumerator to the next element of the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_index == UInt64.MaxValue)
            {
                _index = 0;
                return true;
            }
            else
            {
                ulong index = _index + 1;
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
        }

        public void Reset()
        {
            _index = UInt64.MaxValue;
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
