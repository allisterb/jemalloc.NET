using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Loggers;

namespace jemalloc.Benchmarks
{
    [OrderProvider(methodOrderPolicy: MethodOrderPolicy.Declared)]
    public class FixedBufferVsManagedArrayBenchmark<T> : JemBenchmark<T, int> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public int ArraySize  => Parameter;
        public readonly T fill = typeof(T) == typeof(TestUDT) ? 
            JemUtil.ValToGenericStruct<TestUDT, T>(TestUDT.MakeTestRecord(JemUtil.Rng)) : GM<T>.Random();
        public readonly (T factor, T max) mul = GM<T>.RandomMultiplyFactorAndValue();

        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();
            Info($"Array size is {ArraySize}.");
            T[] managedArray = new T[ArraySize];
            SetValue("managedArray", managedArray);
            FixedBuffer<T> nativeArray = new FixedBuffer<T>(ArraySize);
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
            T[] managedArray = GetValue<T[]>("managedArray");
            T fill = GetValue<T>("fill");
            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = fill;
            }
        }

        [Benchmark(Description = "Fill a FixedBuffer on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillFixedBuffer()
        {
            FixedBuffer<T> nativeArray = GetValue<FixedBuffer<T>>("nativeArray");
            T fill = GetValue<T>("fill");
            nativeArray.Fill(fill);
        }

        [Benchmark(Description = "Create and Fill a managed array with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArrayWithCreate()
        {
            T[] managedArray = new T[ArraySize];
            T fill = GetValue<T>("fill");
            for (int i = 0; i < managedArray.Length; i++)
            {
                managedArray[i] = fill;
            }
            managedArray = null;
        }

        [Benchmark(Description = "Create and Fill a FixedBuffer on the system unmanaged heap with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillFixedBufferWithCreate()
        {
            FixedBuffer<T> nativeArray = new FixedBuffer<T>(ArraySize);
            T fill = GetValue<T>("fill");
            nativeArray.Fill(fill);
            nativeArray.Free();
        }

        [GlobalCleanup(Target = nameof(FillFixedBuffer))]
        public void FillValidateAndCleanup()
        {
            InfoThis();
            T[] managedArray = GetValue<T[]>("managedArray");
            FixedBuffer<T> nativeArray = GetValue<FixedBuffer<T>>("nativeArray");
            T fill = GetValue<T>("fill");
            for (int i = 0; i < ArraySize / 1000; i++)
            {
                int index = GM<T>.Rng.Next(0, ArraySize);
                if (!nativeArray[index].Equals(fill))
                {
                    Log.WriteLineError($"Native array at index {index} is {nativeArray[index]} not {fill}.");
                    throw new Exception();
                }
            }
            nativeArray.Release();
            nativeArray.Free();
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
            this.SetStatistic($"{nameof(ArithmeticMutiplyManagedArray)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
        }

        [Benchmark(Description = "Vector multiply all values of a native array with a single value.")]
        [BenchmarkCategory("Arithmetic")]
        public void ArithmeticMultiplyNativeArray()
        {
            DebugInfoThis();
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            FixedBuffer<T> array = GetValue<FixedBuffer<T>>("nativeArray");
            array.Fill(fill);
            array.VectorMultiply(mul);
            this.SetStatistic($"{nameof(ArithmeticMultiplyNativeArray)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
        }

        [GlobalCleanup(Target = nameof(ArithmeticMultiplyNativeArray))]
        public void ArithmeticMultiplyValidateAndCleanup()
        {
            InfoThis();
            int index; 
            FixedBuffer<T> nativeArray = GetValue<FixedBuffer<T>>("nativeArray");
            T[] managedArray = GetValue<T[]>("managedArray");
            T mul = GetValue<T>("mul");
            T fill = GetValue<T>("fill");
            T val = GM<T>.Multiply(fill, mul);
            for (int i = 0; i < ArraySize % 100; i++)
            {
                index = GM<T>.Rng.Next(0, ArraySize);
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

            }
            managedArray = null;
            nativeArray.Release();
            nativeArray.Free();
            RemoveValue("managedArray");
            RemoveValue("nativeArray");
            RemoveValue("fill");
            RemoveValue("mul");
        }
        #endregion

        #region Create
        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create arrays on the .NET LOH", Baseline = true)]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
        }

        [BenchmarkCategory("Create")]
        [Benchmark(Description = "Create SafeArrays on the system unmanaged heap")]
        public void CreateNativeArray()
        {
            SafeArray<T> array = new SafeArray<T>(ArraySize);

        }
        #endregion

    }
}
