using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public class HugeArray<T> : HugeBuffer<T> where T : struct
    {
        public HugeArray(ulong length) : base(length) { }
    }
}
