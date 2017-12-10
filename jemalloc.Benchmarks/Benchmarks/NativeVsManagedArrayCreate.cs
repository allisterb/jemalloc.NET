using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class NativeVsManagedArrayCreateBenchmark<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [BenchmarkCategory("Native vs managed array")]
        [Benchmark(Description = "Create an array on the .NET managed heap", Baseline = true)]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            someData = null;
        }

        [BenchmarkCategory("Native vs managed array")]
        [Benchmark(Description = "Create a SafeArray on the system unmanaged heap")]
        public void CreateNativeArray()
        {
            SafeArray<T> array = new SafeArray<T>(ArraySize);
        }
    }
}
