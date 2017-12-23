using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class FixedBufferTests : jemallocTest
    {
        [Fact(DisplayName = "Can create a fixed buffer of bytes")]
        public void CanCreateFixedArray()
        {
            FixedBuffer<byte> buffer = new FixedBuffer<byte>(4096);
            SafeArray<FixedBuffer<byte>> byteBuffer = new SafeArray<FixedBuffer<byte>>(1000);
            
        }
    }
}
