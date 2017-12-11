using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class HugeNativeVsManagedArrayFillBenchmark<T> : JemBenchmark<T, ulong> where T : struct
    {
        public ulong ArraySize => Parameter;

        [Benchmark(Description = "Create a managed array and fill with a single value.", Baseline = true)]
        [BenchmarkCategory("Native vs managed array")]
        public void CreateAndFillManagedArray()
        {
            T fill = GetArrayFillValue();
            int size = ArraySize > Int32.MaxValue ? Int32.MaxValue : (int)ArraySize;
            T[] someData = new T[size];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = fill;
            }

            T r = someData[ArraySize / 2];
            someData = null;
        }

        [Benchmark(Description = "Create a HugeArray on the system unmanaged heap and fill with a single value.")]
        [BenchmarkCategory("Native vs managed array")]
        public void CreateAndFillHugeNativeArray()
        {
            T fill = GetArrayFillValue();
            HugeArray<T> array = new HugeArray<T>(ArraySize);
            array.Fill(fill);
            T r = array[ArraySize / 2];
        }
        
    }
}
