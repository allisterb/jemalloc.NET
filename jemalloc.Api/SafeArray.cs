using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace jemalloc
{
    public class SafeArray<T> : SafeBuffer<T> where T : struct
    {
        public SafeArray(int length) : base(length) {}
    }
}
