using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public readonly struct FixedBufferAllocation : IEquatable<FixedBufferAllocation>
    {
        #region Constructor
        public FixedBufferAllocation(IntPtr ptr, ulong size, long timestamp, int tid, int rid)
        {
            this.Ptr = ptr;
            this.Size = size;
            this.TimeStamp = timestamp;
            this.ThreadId = tid;
            this.Rid = rid;
            HashCode = JemUtil.CombineHashCodes(this.Ptr.GetHashCode(), this.Size.GetHashCode(), this.TimeStamp.GetHashCode(), this.ThreadId.GetHashCode(), this.Rid.GetHashCode());
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
        public readonly int Rid;
        public readonly int HashCode;

    }
    public class FixedBufferComparator : IEqualityComparer<FixedBufferAllocation>
    {
        public bool Equals(FixedBufferAllocation l, FixedBufferAllocation r)
        {
            return l.HashCode == r.HashCode;
        }

        public int GetHashCode(FixedBufferAllocation a)
        {
            return a.HashCode;
        }
    }
}
