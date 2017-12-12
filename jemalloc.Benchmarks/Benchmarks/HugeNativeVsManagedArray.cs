using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    [JemBenchmarkJob(ColdStart = false, TargetCount = 1, TimeoutInMinutes = 15)]
    public class HugeNativeVsManagedArrayBenchmark<T> : JemBenchmark<T, ulong> where T : struct
    {
        public ulong ArraySize => Parameter;

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create an array on the .NET managed heap", Baseline = true)]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            someData = null;
        }

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create a HugeArray on the system unmanaged heap")]
        public void CreateHugeNativeArray()
        {
            HugeArray<T> array = new HugeArray<T>(ArraySize);
        }

        [Benchmark(Description = "Create a managed array and fill with a single value.", Baseline = true)]
        [BenchmarkCategory("Native vs managed array")]
        public void FillManagedArray()
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
        public void FillHugeNativeArray()
        {
            T fill = GetArrayFillValue();
            HugeArray<T> array = new HugeArray<T>(ArraySize);
            array.Fill(fill);
            T r = array[ArraySize / 2];
        }
        
    }
}
