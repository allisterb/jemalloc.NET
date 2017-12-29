using System;
using System.Collections.Generic;
using System.Buffers;
using System.Text;

namespace jemalloc
{
    public readonly ref struct MemoryRef<T> where T : struct
    {
        internal MemoryRef(Guid guid, MemoryHandle h)
        {
            Id = guid;
            Handle = h;
        }
        private readonly Guid Id;
        private readonly MemoryHandle Handle;
        public bool Release()
        {
            return false;
        }
    }
}
