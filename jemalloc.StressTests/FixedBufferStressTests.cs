using System;
using Xunit;

namespace jemalloc.StressTests
{
    public class FixedBufferStressTests
    {
        [Fact(DisplayName ="Can allocate fixed buffers")]
        public void Test1()
        {
            int count = 0;
            while (count < 10)
            {
                FixedBuffer<int> b = new FixedBuffer<int>(JemUtil.Rng.Next(100000, 1000000));
                int r = JemUtil.Rng.Next(0, 64);
                b.Fill(r);
                for(int i = 0; i < b.Length; i++)
                {
                    Assert.Equal(r, b[i]);
                }
                Assert.True(b.Free());
                count++;
            }
        }
    }
}
