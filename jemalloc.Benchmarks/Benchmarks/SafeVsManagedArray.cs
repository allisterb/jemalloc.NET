using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Loggers;

namespace jemalloc.Benchmarks
{
    [OrderProvider(methodOrderPolicy: MethodOrderPolicy.Declared)]
    public class SafeVsManagedArrayBenchmark<T> : JemBenchmark<T, int> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public int ArraySize  => Parameter;
        public readonly T fill = typeof(T) == typeof(TestUDT) ? JemUtil.ValToGenericStruct<TestUDT, T>(TestUDT.MakeTestRecord(JemUtil.Rng)) : GM<T>.Random();
        public readonly (T factor, T max) mul = GM<T>.RandomMultiplyFactorAndValue();
      
        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();
            Info($"Array size is {ArraySize}.");
            T[] managedArray = new T[ArraySize];
            SetValue("managedArray", managedArray);
            SafeArray<T> nativeArray = new SafeArray<T>(ArraySize);
            nativeArray.Acquire();
            SetValue("nativeArray", nativeArray);
            if (Operation == Operation.FILL)
            {
                Info($"Array fill value is {fill}.");
                SetValue("fill", fill);
            }
            else if (Operation == Operation.MATH)
            {
                Info($"Array fill value is {mul.max}.");
                nativeArray.Fill(mul.max);
                new Span<T>(managedArray).Fill(mul.max);
                SetValue("fill", mul.max);
                Info($"Array multiply factor is {mul.factor}.");
                SetValue("mul", mul.factor);
            }
          }

        #region Fill      
        [Benchmark(Description = "Fill a managed array with a single value.")]
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

        [Benchmark(Description = "Fill a SafeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillNativeArray()
        {
            DebugInfoThis();
            SafeArray<T> nativeArray = GetValue<SafeArray<T>>("nativeArray");
            T fill = GetValue<T>("fill");
            nativeArray.Fill(fill);
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

        [Benchmark(Description = "Create and Fill a SafeArray on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillNativeArrayWithCreate()
        {
            DebugInfoThis();
            SafeArray<T> nativeArray = new SafeArray<T>(ArraySize);
            T fill = GetValue<T>("fill");
            nativeArray.Fill(fill);
            nativeArray.Close();
        }

        [GlobalCleanup(Target = nameof(FillNativeArrayWithCreate))]
        public void FillArrayValidateAndCleanup()
        {
            InfoThis();
            T[] managedArray = GetValue<T[]>("managedArray");
            SafeArray<T> nativeArray = GetValue<SafeArray<T>>("nativeArray");
            T fill = GetValue<T>("fill");
            int index = JemUtil.Rng.Next(0, ArraySize);
            if (!nativeArray[index].Equals(managedArray[index]))
            {
                Log.WriteLineError($"Native array at index {index} is {nativeArray[index]} not {fill}.");
                throw new Exception();
            }
            nativeArray.Release();
            nativeArray.Close();
            managedArray = null;
            RemoveValue("managedArray");
            RemoveValue("nativeArray");
            RemoveValue("fill");
        }
        #endregion

        #region Arithmetic
        [Benchmark(Description = "Multiply all values of a managed array with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMutiplyManagedArray()
        {
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            T[] m = GetValue<T[]>("managedArray");
            new Span<T>(m).Fill(fill);
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = GM<T>.Multiply(m[i], mul);
            }
        }

        [Benchmark(Description = "Vector multiply all values of a native array with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMultiplyNativeArray()
        {
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            SafeArray<T> array = GetValue<SafeArray<T>>("nativeArray");
            array.Fill(fill);
            array.VectorMultiply(mul);          
        }

        [GlobalCleanup(Target = nameof(ArithmeticMultiplyNativeArray))]
        public void ArithmeticMultiplyValidateAndCleanup()
        {
            InfoThis();
            int index = GM<T>.Rng.Next(0, ArraySize);
            SafeArray<T> nativeArray = GetValue<SafeArray<T>>("nativeArray");
            T[] managedArray = GetValue<T[]>("managedArray");
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            T val = GM<T>.Multiply(fill, mul);
            if (!nativeArray[index].Equals(val))
            {
                Log.WriteLineError($"Native array at index {index} is {nativeArray[index]} not {val}.");
                throw new Exception();
            }
            else if (!managedArray[index].Equals(val))
            {
                Log.WriteLineError($"Managed array at index {index} is {managedArray[index]} not {val}.");
                throw new Exception();
            }
            managedArray = null;
            nativeArray.Release();
            RemoveValue("managedArray");
            RemoveValue("nativeArray");
            RemoveValue("fill");
            RemoveValue("mul");
        }
        #endregion
    }
}
