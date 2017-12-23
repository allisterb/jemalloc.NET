using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public class FixedBufferAllocation
    {
        #region Constructor
        public FixedBufferAllocation(CallerInformation caller, IntPtr ptr, ulong size, long timestamp)
        {
            this.Caller = caller;
            this.Ptr = ptr;
            this.Size = size;
            this.TimeStamp = timestamp;
        }
        #endregion

        public CallerInformation Caller { get; }
        public IntPtr Ptr { get; } = IntPtr.Zero;
        public ulong Size { get; } = 0;
        public long TimeStamp { get; } = 0;
    }
}
