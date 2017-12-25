using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public readonly struct FixedBufferAllocation : IEquatable<FixedBufferAllocation>
    {
        #region Constructor
        public FixedBufferAllocation(IntPtr ptr, ulong size, long timestamp, int tid)
        {
            this.Ptr = ptr;
            this.Size = size;
            this.TimeStamp = timestamp;
            this.ThreadId = tid;
            HashCode = JemUtil.CombineHashCodes(this.Ptr.GetHashCode(), this.Size.GetHashCode(), this.TimeStamp.GetHashCode(), this.ThreadId.GetHashCode());
        }
        #endregion

        public override int GetHashCode()
        {
            return this.HashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is FixedBufferAllocation)
            {
                FixedBufferAllocation o = (FixedBufferAllocation)obj;
                return this.HashCode == o.HashCode;
            }
            else
            {
                return false;
            }
        }

        bool IEquatable<FixedBufferAllocation>.Equals(FixedBufferAllocation other)
        {
            return this.HashCode == other.HashCode;
        }

        public readonly IntPtr Ptr;
        public readonly ulong Size;
        public readonly long TimeStamp;
        public readonly int ThreadId;
        public readonly int HashCode;
    }
}
