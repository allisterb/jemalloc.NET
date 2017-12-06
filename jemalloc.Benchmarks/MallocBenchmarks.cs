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
    public class MallocBenchmarks
    {
        //[Params(10, 510, 1010, 1510, 2010, 2510, 3010, 3510, 4010, 6000, 16000, 26000, 36000, 46000, 56000, 66000, 76000, 86000, 96000)]
        [Params(10, 510)]
        public int ArraySize { get; set; }

        [Benchmark]
        public int AllocManagedArray()
        {
            int value = ArraySize / 2;
            int[] someNumbers = new int[ArraySize];
            for (int i = 0; i < someNumbers.Length; i++)
            {
                someNumbers[i] = value;
            }
            return someNumbers[value];
        }

        /*
        [Benchmark]
        public int Malloc()
        {
            int value = ArraySize / 2;
            NativeArray<int> someNumbers = new NativeArray<int>((uint)ArraySize);
            Span<int> s = someNumbers.Span();
            for (int i = 0; i < someNumbers.Length; i++)
            {
                s[i] = value;
            }
            return s[value];
        }
        */
    }
}
