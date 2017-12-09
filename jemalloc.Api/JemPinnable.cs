using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class JemPinnable<T>
    {
        public T Data;
    }
}
