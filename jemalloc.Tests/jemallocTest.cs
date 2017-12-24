using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public abstract class jemallocTest
    {
        public static Random Rng = new Random();

        public jemallocTest()
        {
            Jem.Init("tcache:false,narenas:3");  
        }

        
    }
}
