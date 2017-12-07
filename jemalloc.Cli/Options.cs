using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace jemalloc.Cli
{
    class Options
    {

    }

    [Verb("malloc", HelpText = "Benchmark data structures backed by native memory allocated using jemalloc vs. .NET managed arrays, vectors, and tensors.")]
    class MallocBenchmarkOptions : Options
    {
        [Option('i', "int", Required = false, HelpText = "Use Int32 integers as the underlying data type.")]
        public bool Integer { get; set; }


        [Value(0, Required = false, HelpText = "The sizes of data structures to .")]
        public IEnumerable<int> Sizes { get; set; }

    }


}
