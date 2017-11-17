using System;
using System.Diagnostics;
using Xunit;

namespace jemalloc.Tests
{
    public class MallCtlTests
    {
        [Fact]
        public void CanReadMallCtl()
        {
            Je.MallocConf = "narenas:3";
            Assert.Equal(3, Je.MallCtl("opt.narenas"));
        }
    }
}
