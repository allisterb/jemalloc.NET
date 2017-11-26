using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc.Buffers
{
    public class JArray<T> : Buffer<T> where T : struct
    {
        public JArray(uint length) : base(length) {}
    }
}
