using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class NativeVsManagedArrayBenchmark<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create an array on the .NET managed heap", Baseline = true)]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            someData = null;
        }

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create a SafeArray on the system unmanaged heap")]
        public void CreateNativeArray()
        {
            SafeArray<T> array = new SafeArray<T>(ArraySize);
        }

        [Benchmark(Description = "Fill a managed array with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArray()
        {
            T fill = GetArrayFillValue();
            T[] someData = new T[ArraySize];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = fill;
            }

            T r = someData[ArraySize / 2];
            someData = null;
        }

        [Benchmark(Description = "Fill a SafeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillNativeArray()
        {
            T fill = GetArrayFillValue();
            SafeArray<T> array = new SafeArray<T>(ArraySize);
            array.Fill(fill);
            T r = array[ArraySize / 2];
        }
    }
}
