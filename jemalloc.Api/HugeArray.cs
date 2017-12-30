using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public class HugeArray<T> : HugeBuffer<T> where T : struct, IEquatable<T>
    {
        public HugeArray(ulong length, params T[] values) : base(length, values) { }
    }
}
