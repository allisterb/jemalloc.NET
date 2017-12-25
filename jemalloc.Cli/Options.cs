using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace jemalloc.Cli
{
    class Options
    {
        [Option('u', "unsigned", Required = false, HelpText = "Use the unsigned version of the underlying data type.")]
        public bool Unsigned { get; set; }

        [Option('b', "int8", Required = false, HelpText = "Use byte as the underlying data type.", SetName = "type")]
        public bool Int8 { get; set; }

        [Option('h', "int16", Required = false, HelpText = "Use Int16 short integer as the underlying data type.", SetName = "type")]
        public bool Int16 { get; set; }

        [Option('i', "int32", Required = false, HelpText = "Use Int32 integer as the underlying data type.", SetName = "type")]
        public bool Int32 { get; set; }

        [Option('l', "int64", Required = false, HelpText = "Use Int64 long integer as the underlying data type.", SetName = "type")]
        public bool Int64 { get; set; }

        [Option('d', "double", Required = false, HelpText = "Use double-precision floating point as the underlying data type.", SetName = "type")]
        public bool Double { get; set; }

        [Option('s', "string", Required = false, HelpText = "Use String as the underlying data type.", SetName = "type")]
        public bool String { get; set; }

        [Option('c', "cold-start", Required = false, HelpText = "Don't run warmup phase of benchmarks.")]
        public bool ColdStart { get; set; }

        [Option('t', "target-count", Required = false, HelpText = "Set the target count of benchmark runs.", Default = 0)]
        public int TargetCount { get; set; }

    }

    [Verb("malloc", HelpText = "Benchmark native memory allocation using jemalloc vs. .NET managed heap and Large Object Heap allocation")]
    class MallocBenchmarkOptions : Options
    {
        [Option("create", Required = false, HelpText = "Benchmark malloc and Span<T> creation vs managed array creation.")]
        public bool Create { get; set; }

        [Option("fill", Required = false, HelpText = "Benchmark fill Span<T> on system unmanaged heap vs fill managed arrays.")]
        public bool Fill { get; set; }

        [Value(0, Required = true, HelpText = "The sizes of data structures to benchmark.")]
        public IEnumerable<int> Sizes { get; set; }
    }

    [Verb("buffer", HelpText = "Benchmark FixedBuffer arrays backed by native memory allocated using jemalloc vs. .NET managed arrays.")]
    class FixedBufferBenchmarkOptions : Options
    {
        [Option("create", Required = false, HelpText = "Benchmark native array creation vs managed arrays.")]
        public bool Create { get; set; }

        [Option("fill", Required = false, HelpText = "Benchmark fill native array vs managed arrays.")]
        public bool Fill { get; set; }

        [Option("math", Required = false, HelpText = "Benchmark arithmetic and other math operations on native array vs managed arrays.")]
        public bool Math { get; set; }

        [Value(0, Required = true, HelpText = "The sizes of data structures to benchmark.")]
        public IEnumerable<int> Sizes { get; set; }
    }

    [Verb("sarray", HelpText = "Benchmark arrays backed by native memory allocated using jemalloc vs. .NET managed arrays.")]
    class SafeArrayBenchmarkOptions : Options
    {
        [Option("create", Required = false, HelpText = "Benchmark native array creation vs managed arrays.")]
        public bool Create { get; set; }

        [Option("fill", Required = false, HelpText = "Benchmark fill native array vs managed arrays.")]
        public bool Fill { get; set; }

        [Option("math", Required = false, HelpText = "Benchmark arithmetic and other math operations on native array vs managed arrays.")]
        public bool Math { get; set; }

        [Value(0, Required = true, HelpText = "The sizes of data structures to benchmark.")]
        public IEnumerable<int> Sizes { get; set; }
    }



    [Verb("hugearray", HelpText = "Benchmark huge arrays backed by native memory allocated using jemalloc vs. .NET managed arrays.")]
    class HugeNativeArrayBenchmarkOptions : Options
    {
        [Option("create", Required = false, HelpText = "Benchmark huge native array creation vs managed arrays.")]
        public bool Create { get; set; }

        [Option("fill", Required = false, HelpText = "Benchmark huge native array fill vs managed arrays.")]
        public bool Fill { get; set; }

        [Option("math", Required = false, HelpText = "Benchmark arithmetic and other math operations on native array vs managed arrays.")]
        public bool Math { get; set; }

        [Value(0, Required = true, HelpText = "The sizes of data structures to benchmark.")]
        public IEnumerable<ulong> Sizes { get; set; }
    }
}
