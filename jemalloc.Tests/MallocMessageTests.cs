using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class MallocMessageTests : jemallocTest
    {
        [Fact]
        public void CanPrintMallocStats()
        {
            Assert.True(Je.MallocStatsPrint().Contains("opt.narenas: 3"));
        }
    }
}
