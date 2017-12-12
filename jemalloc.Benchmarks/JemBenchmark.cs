using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BenchmarkDotNet;
using BenchmarkDotNet.Order;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Code;

namespace jemalloc.Benchmarks
{
    [JemBenchmarkJob]
    [MemoryDiagnoser]
    public class JemBenchmark<TData, TParam> where TData : struct where TParam : struct
    {
        public JemBenchmark() {}

        [ParamsSource(nameof(GetParameters))]
        public TParam Parameter;

        public IEnumerable<IParam> GetParameters() => BenchmarkParameters.Select(p => new JemBenchmarkParam<TParam>(p));

        public static IEnumerable<TParam> BenchmarkParameters { get; set; }

        public static int GetBenchmarkMethodCount<TBench>() where TBench : JemBenchmark<TData, TParam>
        {
            return typeof(TBench).GenericTypeArguments.First().GetMethods(BindingFlags.Public).Count();
        }

        public static void SetColdStartOverride(bool value)
        {
            JemBenchmarkJobAttribute.ColdStartOverride = value;
        }
        public static unsafe TData GetArrayFillValue()
        {
            TData value = default;
            switch (value)
            {
                case Byte v:
                    return JemUtil.ValToGenericStruct<Byte, TData>(Byte.MaxValue / 2);
                    
                case SByte v:
                    return JemUtil.ValToGenericStruct<SByte, TData>(SByte.MaxValue / 2);

                case UInt16 v:
                    return JemUtil.ValToGenericStruct<UInt16, TData>(UInt16.MaxValue / 2);

                case Int16 v:
                    return JemUtil.ValToGenericStruct<Int16, TData>(Int16.MaxValue / 2);

                case UInt32 v:
                    return JemUtil.ValToGenericStruct<UInt32, TData>(UInt32.MaxValue / 2);

                case Int32 v:
                    return JemUtil.ValToGenericStruct<Int32, TData>(Int32.MaxValue / 2);

                case UInt64 v:
                    return JemUtil.ValToGenericStruct<UInt64, TData>(UInt64.MaxValue / 2);

                case Int64 v:
                    return JemUtil.ValToGenericStruct<Int64, TData>(Int64.MaxValue / 2);

                default:
                    return value;
            }
        }
    }
}
