using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class MallocConfTests : jemallocTest
    {
        public MallocConfTests() : base() {}

        [Fact]
        public void CanGetConf()
        {
            Assert.Equal("tcache:false,narenas:3", Jem.MallocConf);
        }
    }
}
