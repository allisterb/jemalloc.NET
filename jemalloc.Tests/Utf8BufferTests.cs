using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class Utf8BufferTests : jemallocTest
    {
        [Fact(DisplayName = "Can construct Utf8Buffer")]
        public void CanConstructUtf8String()
        {
            Utf8Buffer s = new Utf8Buffer("Hello World");
            Assert.Equal(6, s.IndexOf("W"));
        }
    }
}
