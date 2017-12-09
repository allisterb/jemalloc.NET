using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
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
            return typeof(TBench).GetMethods(BindingFlags.Public).Where(m => m.GetCustomAttribute(typeof(BenchmarkAttribute)) != null).Count();
        }

    }
}
