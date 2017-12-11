using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace jemalloc
{
    public class SafeArray<T> : SafeBuffer<T> where T : struct
    {
        public SafeArray(int length, params T[] values) : base(length, values) {}
    }
}
