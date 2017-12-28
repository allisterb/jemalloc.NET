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
            FixedBuffer<T> nativeArray = new FixedBuffer<T>(ArraySize);
            nativeArray.Acquire();
            SetValue("nativeArray", nativeArray);
        }

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

        [GlobalCleanup(Target = nameof(FillFixedBufferWithCreate))]
        public void ValidateAndCleanupFillArray()
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
        [GlobalSetup(Target = nameof(ArithmeticMutiplyManagedArray))]
        public void ArithmeticMutiplyGlobalSetup()
        {
            InfoThis();
            (T mul, T fill) = GM<T>.RandomMultiplyFactorAndValue();
            FixedBuffer<T> na = new FixedBuffer<T>(ArraySize);
            T[] ma = new T[ArraySize];
            Info($"Array fill value is {fill}.");
            Info($"Array mul value is {mul}.");
            na.Fill(fill);
            for (int i = 0; i < ma.Length; i++)
            {
                ma[i] = fill;
            }
            na.Acquire();
            SetValue("fill", fill);
            SetValue("mul", mul);
            SetValue("managedArray", ma);
            SetValue("nativeArray", na);
        }       

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
        public void ArithmeticMultiplyCleanup()
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
