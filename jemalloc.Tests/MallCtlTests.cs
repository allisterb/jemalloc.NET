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
            Assert.Equal(true, Je.GetMallCtlBool("config.debug"));
            Assert.Equal(false, Je.GetMallCtlBool("config.valgrind"));
        }

        [Fact]
        public void CanReadMallCtlStr()
        {
            string version = Je.GetMallCtlStr("version");
            Assert.True(version.StartsWith("5"));
        }
    }
}
