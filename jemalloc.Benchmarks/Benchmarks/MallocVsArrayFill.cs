using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class MallocVsArrayFillBenchmark<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [Benchmark(Description = "Create array of data on the managed heap and fill with a single value.", Baseline = true)]
        public void CreateAndFillManagedArray()
        {
            T fill = GetArrayFillValue();
            T[] someData = new T[ArraySize];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = fill;
            }
            T r =  someData[ArraySize / 2];
        }

        [Benchmark(Description = "Allocate memory on the system unmanaged heap with access via Span<T> and fill with a single value.")]
        public void MallocAndFillSpan()
        {
            T fill = GetArrayFillValue();
            ulong msize = (ulong)(ArraySize * JemUtil.SizeOfStruct<T>());
            IntPtr ptr = Jem.Malloc(msize);
            Span<T> s = JemUtil.PtrToSpan<T>(ptr, ArraySize);
            s.Fill(fill);
            T r = s[ArraySize / 2];
            Jem.Free(ptr);
        }
    }
}
