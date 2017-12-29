using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class FixedUtf8BufferTests : jemallocTest
    {
        [Fact(DisplayName = "Can construct FixedUtf8String")]
        public void CanConstructUtf8String()
        {
            FixedUtf8String s = new FixedUtf8String("Hello World");
            Assert.Equal(6, s.IndexOf("W"));
        }
    }
}
