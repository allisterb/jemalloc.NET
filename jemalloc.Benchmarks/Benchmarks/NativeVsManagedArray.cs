using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    public class NativeVsManagedArrayBenchmark<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [GlobalSetup]
        public void GlobalSetup()
        {
            managedArray = new T[ArraySize];
            nativeArray = new SafeArray<T>(ArraySize);
            fill = GetArrayFillValue();
            mul = GetArrayMulValue();
            Console.WriteLine("Managed array fill value is {0}.", fill);
            Console.WriteLine("Multiply factor is {0}", mul);
        }

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
            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = fill;
            }

            T r = managedArray[ArraySize / 2];
        }

        [Benchmark(Description = "Fill a SafeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillNativeArray()
        {
            nativeArray.Fill(fill);
            T r = nativeArray[ArraySize / 2];
        }

        [Benchmark(Description = "Multiply all values of a managed array with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMutiplyManagedArray()
        {
            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = JemUtil.GenericMultiply(managedArray[i], mul);
            }
            T r = managedArray[ArraySize / 2];
        }

        [Benchmark(Description = "Vector multiply all values of a native array with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMultiplyNativeArray()
        {
            nativeArray.VectorMultiply(mul);
            T r = nativeArray[ArraySize / 2];
        }

        protected T[] managedArray;
        protected SafeArray<T> nativeArray;
        protected T fill;
        protected T mul;
    }
}
