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
            Assert.Equal(3, Je.GetMallCtlInt32("opt.narenas"));
        }

        [Fact]
        public void CanReadMallCtlBool()
        {
            Assert.True(Je.GetMallCtlBool("config.debug"));
            Assert.False(Je.GetMallCtlBool("config.valgrind"));
        }

        [Fact]
        public void CanReadMallCtlStr()
        {
            Assert.StartsWith("5", Je.GetMallCtlStr("version"));
        }
    }
}
