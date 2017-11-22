using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public abstract class jemallocTest
    { 
        public jemallocTest()
        {
            Je.Init("tcache:false,narenas:3");  
        }
    }
}
