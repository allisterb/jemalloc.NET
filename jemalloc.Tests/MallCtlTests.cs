using System;
using System.Diagnostics;
using Xunit;

namespace jemalloc.Tests
{
    public class MallCtlTests
    {
        [Fact]
        public void CanReadMallCtlInt32()
        {
            Assert.Equal(32, Je.GetMallCtlInt32("opt.narenas"));
        }

        [Fact]
        public void CanReadMallCtlBool()
        {
            Assert.Equal(true, Je.GetMallCtlBool("config.debug"));
        }

        [Fact]
        public void CanReadMallCtlStr()
        {
            string version = Je.GetMallCtlStr("version");
            Assert.True(version.StartsWith("5"));
        }
    }
}
