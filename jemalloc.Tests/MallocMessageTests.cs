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
            Assert.Contains(Je.MallocStatsPrint(), "opt.narenas: 3");
        }
    }
}
