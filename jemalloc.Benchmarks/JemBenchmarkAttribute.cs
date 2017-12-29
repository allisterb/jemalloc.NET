using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Configs;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class JemBenchmarkAttribute : BenchmarkAttribute
    {
        public static Category Category { get; set; }

        public static Operation Operation { get; set; }
  
        public static IConfig CurrentConfig { get; set; }
    }
}
