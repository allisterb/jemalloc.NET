using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace jemalloc
{
    public class NativeArray<T> : Buffer<T> where T : struct
    {
        public NativeArray(uint length) : base(length) {}

        public NativeArray(int length) : base((uint) length) {}
    }
}
