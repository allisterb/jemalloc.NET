using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    [JemBenchmarkJob(BenchmarkDotNet.Engines.RunStrategy.ColdStart, 7)]
    public class HugeNativeVsManagedArrayBenchmark<T> : JemBenchmark<T, ulong> where T : struct
    {
        public ulong ArraySize => Parameter;

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create an array with the maximum size [2146435071] on the .NET managed heap")]
        public void CreateManagedArray()
        {
            T[] someData = new T[2146435071];
            someData = null;
        }

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create a HugeArray on the system unmanaged heap")]
        public void CreateHugeNativeArray()
        {
            HugeArray<T> array = new HugeArray<T>(ArraySize);
        }

        [Benchmark(Description = "Fill a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Native vs managed array")]
        public void FillManagedArray()
        {
            T fill = GetArrayFillValue();
            T[] someData = new T[2146435071];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = fill;
            }

            //T r = someData[ArraySize / 2];
            someData = null;
        }

        [Benchmark(Description = "Fill a HugeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Native vs managed array")]
        public void FillHugeNativeArray()
        {
            T fill = GetArrayFillValue();
            HugeArray<T> array = new HugeArray<T>(ArraySize);
            array.Fill(fill);
            T r = array[ArraySize / 2];
        }
        
    }
}
