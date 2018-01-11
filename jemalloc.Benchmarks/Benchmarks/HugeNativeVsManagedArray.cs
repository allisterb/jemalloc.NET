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
            T[] managedArray = new T[MaxManagedArraySize];
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

        [GlobalCleanup(Target = nameof(FillHugeNativeArray))]
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


    }
}
