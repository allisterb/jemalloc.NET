using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class NativeVsManagedArrayFillBenchmark<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize => Parameter;

        [Benchmark(Description = "Create a managed array and fill with a single value.", Baseline = true)]
        [BenchmarkCategory("Native vs managed array")]
        public void CreateAndFillManagedArray()
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

        [Benchmark(Description = "Create a SafeArray on the system unmanaged heap and fill with a single value.")]
        [BenchmarkCategory("Native vs managed array")]
        public void CreateAndFillNativeArray()
        {
            T fill = GetArrayFillValue();
            SafeArray<T> array = new SafeArray<T>(ArraySize);
            array.Fill(fill);
            T r = array[ArraySize / 2];
        }
        
    }
}
