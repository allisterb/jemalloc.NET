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

        [Option('f', "float", Required = false, HelpText = "Use single-precision floating point as the underlying data type.", SetName = "type")]
        public bool Float { get; set; }

        [Option('m', "double", Required = false, HelpText = "Use double-precision floating point as the underlying data type.", SetName = "type")]
        public bool Double { get; set; }

        [Option('s', "string", Required = false, HelpText = "Use String as the underlying data type.", SetName = "type")]
        public bool String { get; set; }

        [Option("udt", Required = false, HelpText = "Use a user-defined data type as the underlying data type.", SetName = "type")]
        public bool Udt { get; set; }

        [Option('c', "cold-start", Required = false, HelpText = "Don't run warmup phase of benchmarks.")]
        public bool ColdStart { get; set; }

        [Option("once", Required = false, HelpText = "Run 1 iteration of benchmarks.")]
        public bool Once { get; set; }

        [Option('t', "target-count", Required = false, HelpText = "Set the target count of benchmark runs.", Default = 0)]
        public int TargetCount { get; set; }

        [Option('d', "debug", Required = false, HelpText = "Run benchmarks in debug mode.")]
        public bool Debug { get; set; }

    }

    [Verb("malloc", HelpText = "Benchmark native memory allocation using jemalloc vs. .NET managed heap and Large Object Heap allocation.")]
    class MallocBenchmarkOptions : Options
    {
        [Option("create", Required = false, HelpText = "Benchmark malloc vs managed array alloc.")]
        public bool Create { get; set; }

        [Option("fill", Required = false, HelpText = "Benchmark fill Span<T> on system unmanaged heap vs fill managed arrays.")]
        public bool Fill { get; set; }

        [Option("fragment", Required = false, HelpText = "Run an allocation pattern that fragments the LOH vs. native memory.")]
        public bool Fragment { get; set; }

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

    [Verb("safe", HelpText = "Benchmark SafeArray arrays backed by native memory allocated using jemalloc vs. .NET managed arrays.")]
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

    [Verb("huge", HelpText = "Benchmark HugeArray arrays backed by native memory allocated using jemalloc vs. .NET managed arrays.")]
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

    [Verb("vector", HelpText = "Benchmark SIMD vectorized algorithms on native memory data structures vs. SIMD using .NET managed arrays.")]
    class VectorBenchmarkOptions : Options
    {
        [Option("mandel", Required = false, HelpText = "Benchmark Mandelbrot bitmap generation.")]
        public bool Mandelbrot { get; set; }

        [Value(0, Required = true, HelpText = "The sizes of data structures to benchmark.")]
        public IEnumerable<ulong> Sizes { get; set; }
    }
}
