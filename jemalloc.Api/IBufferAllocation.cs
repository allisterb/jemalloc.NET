using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public interface IBufferAllocation
    {
        IntPtr Ptr { get; }
        ulong Size { get; }
        long TimeStamp { get; }
        int ThreadId { get; }
        int HashCode { get; }
    }
}
