using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace jemalloc.Benchmarks
{
    [OrderProvider(methodOrderPolicy: MethodOrderPolicy.Declared)]
    public class HugeNativeVsManagedArrayBenchmark<T> : JemBenchmark<T, ulong> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public ulong ArraySize => Parameter;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            base.GlobalSetup();
            Info($"Array size is {ArraySize}.");
        }

        #region Fill
        [GlobalSetup(Target = nameof(FillManagedArray))]
        public void FillSetup()
        {
            InfoThis();
            T fill = GM<T>.Random();
            Info($"Array fill value is {fill}.");
            SetValue("fill", fill);
            SetValue("managedArray", new T[ArraySize]);
            HugeArray<T> hugeArray = new HugeArray<T>(ArraySize);
            hugeArray.Acquire();
            SetValue("hugeArray", hugeArray);
        }

        [Benchmark(Description = "Fill a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArray()
        {
            InfoThis();
            T[] managedArray = GetValue<T[]>("managedArray");
            T fill = GetValue<T>("fill");
            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = fill;
            }
        }

        [Benchmark(Description = "Fill a HugeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillHugeNativeArray()
        {
            InfoThis();
            HugeArray<T> hugeArray = GetValue<HugeArray<T>>("hugeArray");
            T fill = GetValue<T>("fill");
            hugeArray.Fill(fill);
        }

        [Benchmark(Description = "Create and Fill a managed array with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArrayWithCreate()
        {
            InfoThis();
            T[] managedArray = new T[ArraySize];
            T fill = GetValue<T>("fill");
            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = fill;
            }
            managedArray = null;
        }

        [Benchmark(Description = "Create and Fill a HugeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillHugeNativeArrayWithCreate()
        {
            InfoThis();
            HugeArray<T> hugeArray = new HugeArray<T>(ArraySize);
            T fill = GetValue<T>("fill");
            hugeArray.Fill(fill);
            hugeArray.Close();
            hugeArray = null;
        }

        [GlobalCleanup(Target = nameof(FillHugeNativeArrayWithCreate))]
        public void CleanupFillArray()
        {
            InfoThis();
            T[] managedArray = GetValue<T[]>("managedArray");
            HugeArray<T> hugeArray = GetValue<HugeArray<T>>("hugeArray");
            T fill = GetValue<T>("fill");
            ulong index = GM<UInt64>.Random(ArraySize);
            if (!hugeArray[index].Equals(fill))
            {
                Error($"Native array at index {index} is {hugeArray[index]} not {fill}.");
                throw new Exception();
            }
            hugeArray.Release();
            managedArray = null;
            RemoveValue("managedArray");
            RemoveValue("hugeArray");
            RemoveValue("fill");
        }

        #endregion

        #region Create
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
            array.Release();
        }
        #endregion

        #region Arithmetic
        [GlobalSetup(Target = nameof(ArithmeticMutiplyManagedArray))]
        public void ArithmeticMutiplyGlobalSetup()
        {
            InfoThis();
            (T mul, T fill) = GM<T>.RandomMultiplyFactorAndValue();
            HugeArray<T> ha = new HugeArray<T>(ArraySize);
            T[] ma = new T[ArraySize];
            Info($"Array fill value is {fill}.");
            Info($"Array mul value is {mul}.");
            ha.Fill(fill);
            for (int i = 0; i < ma.Length; i++)
            {
                ma[i] = fill;
            }
            ha.Acquire();
            SetValue("fill", fill);
            SetValue("mul", mul);
            SetValue("managedArray", ma);
            SetValue("hugeArray", ha);
        }

        [Benchmark(Description = "Multiply all values of a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMutiplyManagedArray()
        {
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            T[] managedArray = GetValue<T[]>("managedArray");

            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = GM<T>.Multiply(managedArray[i], mul);
            }
            T r = managedArray[ArraySize / 2];
        }

        [Benchmark(Description = "Vector multiply all values of a native array with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMultiplyHugeNativeArray()
        {
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            HugeArray<T> hugeArray = GetValue<HugeArray<T>>("hugeArray");
            hugeArray.Fill(fill);
            hugeArray.VectorMultiply(mul);
        }

        [GlobalCleanup(Target = nameof(ArithmeticMultiplyHugeNativeArray))]
        public void ArithmeticMultiplyCleanup()
        {
            InfoThis();
            ulong index = (ulong) (GM<T>.Rng.NextDouble() * ArraySize);
            HugeArray<T> hugeArray = GetValue<HugeArray<T>>("hugeArray");
            T[] managedArray = GetValue<T[]>("managedArray");
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            T val = GM<T>.Multiply(fill, mul);
            if (!hugeArray[index].Equals(val))
            {
                Error($"Native array at index {index} is {hugeArray[index]} not {val}.");
                throw new Exception();
            }
            else if (!managedArray[index].Equals(val))
            {
                Error($"Managed array at index {index} is {managedArray[index]} not {val}.");
                throw new Exception();
            }
            managedArray = null;
            hugeArray.Release();
            RemoveValue("managedArray");
            RemoveValue("hugeArray");
            RemoveValue("fill");
            RemoveValue("mul");
        }
        #endregion
    }
}
