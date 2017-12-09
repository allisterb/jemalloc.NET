using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;

namespace jemalloc.Benchmarks
{
    
    public class MallocVsArrayBenchmarks<T> : JemBenchmark<T, int> where T : struct
    {
        public int ArraySize  => Parameter;

        [Benchmark(Description = "Create array of data on the managed heap and fill with a single value.", Baseline = true)]
        public void CreateAndFillManagedArray()
        {
            T fill = GetFillVaue();
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
            T fill = GetFillVaue();
            ulong msize = (ulong)(ArraySize * JemUtil.SizeOfStruct<T>());
            IntPtr ptr = Jem.Malloc(msize);
            Span<T> s = JemUtil.PtrToSpan<T>(ptr, ArraySize);
            s.Fill(fill);
            T r = s[ArraySize / 2];
            Jem.Free(ptr);
        }

        protected unsafe T GetFillVaue()
        {
            T value = default;
            switch (value)
            {
                case Byte v:
                    v = (byte)(Byte.MaxValue / 2);
                    void* u8ptr = &v;
                    T u8ret = JemUtil.PtrToStruct<T>(u8ptr);
                    return u8ret;

                case SByte v:
                    v = (sbyte)(SByte.MaxValue / 2);
                    void* s8ptr = &v;
                    T s8ret = JemUtil.PtrToStruct<T>(s8ptr);
                    return s8ret;

                case UInt16 v:
                    v = (ushort)(UInt16.MaxValue / 2);
                    void* u16ptr = &v;
                    T u16ret = JemUtil.PtrToStruct<T>(u16ptr);
                    return u16ret;

                case Int16 v:
                    v = (short)(Int16.MaxValue / 2);
                    void* s16ptr = &v;
                    T s16ret = JemUtil.PtrToStruct<T>(s16ptr);
                    return s16ret;

                case UInt32 v:
                    v = (uint)(UInt32.MaxValue / 2);
                    void* u32ptr = &v;
                    T u32ret = JemUtil.PtrToStruct<T>(u32ptr);
                    return u32ret;

                case Int32 v:
                    v = (int)(Int32.MaxValue / 2);
                    void* s32ptr = &v;
                    T s32ret = JemUtil.PtrToStruct<T>(s32ptr);
                    return s32ret;

                case UInt64 v:
                    v = (ulong)(UInt64.MaxValue / 2);
                    void* u64ptr = &v;
                    T u64ret = JemUtil.PtrToStruct<T>(u64ptr);
                    return u64ret;

                case Int64 v:
                    v = (long)(Int64.MaxValue / 2);
                    void* s64ptr = &v;
                    T s64ret = JemUtil.PtrToStruct<T>(s64ptr);
                    return s64ret;

                default:
                    return value;
            }

        }
    }
}
