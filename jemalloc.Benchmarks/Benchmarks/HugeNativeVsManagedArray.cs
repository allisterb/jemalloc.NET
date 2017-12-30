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
        public readonly int MaxManagedArraySize = 2146435071;
        public T fill = typeof(T) == typeof(TestUDT) ? JemUtil.ValToGenericStruct<TestUDT, T>(TestUDT.MakeTestRecord(JemUtil.Rng)) : GM<T>.Random();
        public (T factor, T max) mul = GM<T>.RandomMultiplyFactorAndValue();

        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();
            Info($"Unmanaged array size is {ArraySize}.");
            Info($"Managed array size is {MaxManagedArraySize}.");
            T[] managedArray = new T[MaxManagedArraySize];
            SetValue("managedArray", managedArray);
            HugeArray<T> hugeArray = new HugeArray<T>(ArraySize);
            hugeArray.Acquire();
            SetValue("hugeArray", hugeArray);
            if (Operation == Operation.FILL)
            {
                SetValue("fill", fill);
                Info($"Array fill value is {fill}.");
                SetValue("fill", fill);
            }
            else if (Operation == Operation.MATH)
            {
                Info($"Array fill value is {mul.max}.");
                new Span<T>(managedArray).Fill(mul.max);
                hugeArray.Fill(mul.max);
                SetValue("fill", mul.max);
                Info($"Array multiply factor is {mul.factor}.");
                SetValue("mul", mul.factor);
            }
        }

        #region Fill
        [Benchmark(Description = "Fill a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArray()
        {
            DebugInfoThis();
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
            DebugInfoThis();
            HugeArray<T> hugeArray = GetValue<HugeArray<T>>("hugeArray");
            T fill = GetValue<T>("fill");
            hugeArray.Fill(fill);
        }

        [Benchmark(Description = "Create and Fill a managed array with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArrayWithCreate()
        {
            DebugInfoThis();
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
            DebugInfoThis();
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
        [Benchmark(Description = "Multiply all values of a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMultiplyManagedArray()
        {
            DebugInfoThis();
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
            DebugInfoThis();
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            HugeArray<T> hugeArray = GetValue<HugeArray<T>>("hugeArray");
            hugeArray.Fill(fill);
            hugeArray.VectorMultiply(mul);
        }

        [GlobalCleanup(Target = nameof(ArithmeticMultiplyHugeNativeArray))]
        public void ArithmeticMultiplyValidateAndCleanup()
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
            hugeArray.Close();
            RemoveValue("managedArray");
            RemoveValue("hugeArray");
            RemoveValue("fill");
            RemoveValue("mul");
        }
        #endregion
    }
}
