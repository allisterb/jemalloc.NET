using System;
using System.Collections.Generic;

using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;

namespace jemalloc.Benchmarks
{
    
    public class MallocVsArrayBenchmarks<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [Benchmark(Description = "Create array of data on the managed heap and fill with a single value.", Baseline = true)]
        public void CreateAndFillManagedArray()
        {
            T value = default;
            T[] someData = new T[ArraySize];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = value;
            }
            T r =  someData[ArraySize / 2];
        }

        
        [Benchmark(Description = "Allocate memory on the system unmanaged heap with access via Span<T> and fill with a single value.")]
        public void MallocAndFillSpan()
        {
            int value = ArraySize / 2;
            ulong msize = (ulong)(ArraySize * sizeof(int));
            IntPtr ptr = Jem.Malloc(msize);
            Span<int> s = Jem.GetSpanFromPtr<int>(ptr, ArraySize);
            
            s.Fill(value);
            int r = s[value];
            Jem.Free(ptr);
        }
        
    }
}
