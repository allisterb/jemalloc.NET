using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class BufferVsManagedArrayCreateBenchmark<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [BenchmarkCategory("Buffer vs managed array")]
        [Benchmark(Description = "Create an array on the .NET managed heap", Baseline = true)]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            someData = null;
        }

        [BenchmarkCategory("Buffer vs managed array")]
        [Benchmark(Description = "Create a Buffer on the system unmanaged heap")]
        public void CreateBuffer()
        {
            Buffer<int> buffer = new Buffer<int>(ArraySize);
            buffer.Release();
             
        }
    }
}
