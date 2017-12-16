using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace jemalloc.Benchmarks
{
    [JemBenchmarkJob(BenchmarkDotNet.Engines.RunStrategy.ColdStart, 7)]
    public class HugeNativeVsManagedArrayBenchmark<T> : JemBenchmark<T, ulong> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public ulong ArraySize => Parameter;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            base.GlobalSetup();
            managedArray = new T[2146435071];
            nativeArray = new HugeArray<T>(ArraySize);
            fill = GetArrayFillValue();
            mul = GetArrayMulValue();
            Console.WriteLine("Managed array fill value is {0}.", fill);
            Console.WriteLine("Multiply factor is {0}", mul);
        }

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create an array with the maximum size [2146435071] on the .NET managed heap")]
        public void CreateManagedArray()
        {
            T[] someData = new T[2146435071];
        }

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create a HugeArray on the system unmanaged heap")]
        public void CreateHugeNativeArray()
        {
            HugeArray<T> array = new HugeArray<T>(ArraySize);
            array.Close();
        }

        [Benchmark(Description = "Fill a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Fiill")]
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
        [BenchmarkCategory("Fill")]
        public void FillHugeNativeArray()
        {
            T fill = GetArrayFillValue();
            HugeArray<T> array = new HugeArray<T>(ArraySize);
            array.Fill(fill);
            T r = array[ArraySize / 2];
        }

        [Benchmark(Description = "Multiply all values of a managed array with the maximum size [2146435071] with a single value.")]
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

        #region Fields
        protected T[] managedArray;
        protected HugeArray<T> nativeArray;
        protected T fill;
        protected T mul;
        #endregion

    }
}
