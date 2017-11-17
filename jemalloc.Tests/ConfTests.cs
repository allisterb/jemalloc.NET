using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class ConfTests
    {
        [Fact]
        public void CanGetConf()
        {
            Je.MallocConf = "narenas:3";
            Assert.Equal("narenas:3", Je.MallocConf);
        }
    }
}
