using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;


namespace jemalloc.Benchmarks.Benchmarks
{
    public class VectorVsISPC : JemBenchmark<float, int>
    {
        public int ArraySize => Parameter;

        #region Mandelbrot
        #endregion
    }
}
