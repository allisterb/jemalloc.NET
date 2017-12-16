using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class MallocVsArrayBenchmark<T> : JemBenchmark<T, int> where T : struct, IEquatable<T>
    {
        public int ArraySize  => Parameter;

        [Benchmark(Description = "Create array of data on the managed heap.")]
        [BenchmarkCategory("Create")]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            someData = null;
         }


        [Benchmark(Description = "Fill array of data on the managed heap.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArray()
        {
            T fill = GetArrayFillValue();
            T[] someData = new T[ArraySize];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = fill;
            }
            T r =  someData[ArraySize / 2];
        }

        [Benchmark(Description = "Malloc buffer and Span<T> on the system managed heap.")]
        [BenchmarkCategory("Create")]
        public void CreateSpan()
        {
            ulong msize = (ulong)(ArraySize * JemUtil.SizeOfStruct<T>());
            IntPtr ptr = Jem.Malloc(msize);
            unsafe
            {
                Span<T> s = new Span<T>(ptr.ToPointer(), ArraySize);
            }
            Jem.Free(ptr);
        }

        [Benchmark(Description = "Fill memory on the system unmanaged heap using Span<T> with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillSpan()
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
