using System;
using System.Diagnostics;
using Xunit;

namespace jemalloc.Tests
{
    public class MallCtlTests : jemallocTest
    {
        public MallCtlTests() : base() {}
        [Fact]
        public void CanReadMallCtlInt32()
        {
            Assert.Equal(3, Jem.GetMallCtlInt32("opt.narenas"));
        }

        [Fact]
        public void CanReadMallCtlBool()
        {
            Assert.True(Jem.GetMallCtlBool("config.debug"));
            Assert.False(Jem.GetMallCtlBool("config.xmalloc"));
        }

        [Fact]
        public void CanReadMallCtlStr()
        {
            Assert.StartsWith("5", Jem.GetMallCtlStr("version"));
        }
    }
}
