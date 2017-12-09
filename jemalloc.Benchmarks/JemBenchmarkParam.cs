using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Code;
using BenchmarkDotNet.Parameters;

namespace jemalloc.Benchmarks
{
    public class JemBenchmarkParam<T> : IParam where T : struct
    {
        private static string ctorName = typeof(T).Name;

        private readonly T value;

        public JemBenchmarkParam(T value) => this.value = value;

        public object Value => value;

        public string DisplayText => $"{value}";

        public string ToSourceCode() => $"new {ctorName}({value})";
    }
}
