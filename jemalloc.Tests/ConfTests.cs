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
            Je.SetMallocConf("narenas:3");
            string c = Je.GetMallocConf();
        }
    }
}
