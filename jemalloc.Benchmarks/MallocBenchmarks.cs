using System;
using System.IO;

using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;

namespace jemalloc.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    public class MallocVsArrayBenchmarks
    {
        [Params(100000, 200000, 500000, 1000000)]
        public int ArraySize { get; set; }

        [Benchmark(Description = "Create array of integers on the managed heap and fill with a single value.", Baseline = true)]
        public void CreateAndFillManagedArray()
        {
            int value = ArraySize / 2;
            int[] someNumbers = new int[ArraySize];
            for (int i = 0; i < someNumbers.Length; i++)
            {
                someNumbers[i] = value;
            }
            int r =  someNumbers[value];
        }

        
        [Benchmark(Description = "Allocate memory on the system unmanaged heap with access via Span<int> and fill with a single value.")]
        public void MallocAndFillSpan()
        {
            int value = ArraySize / 2;
            ulong msize = (ulong)(ArraySize * sizeof(int));
            IntPtr ptr = Je.Malloc(msize);
            Span<int> s = Je.GetSpanFromPtr<int>(ptr, ArraySize);
            
            s.Fill(value);
            int r = s[value];
            Je.Free(ptr);
        }
        
    }
}
