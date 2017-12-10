using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class BufferTests : jemallocTest
    {
        [Fact]
        public void CanConstructBuffer()
        {
            Buffer<int> buffer = new Buffer<int>(1000000000);
            buffer[32] = 12;
            Assert.Equal(12, buffer[32]);
            
        }
    }
}
