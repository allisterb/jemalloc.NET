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

    [Verb("malloc", HelpText = "Allocate memory and print process statistics.")]
    class MallocOptions : Options
    {
        [Option('s', "size", Required = true, HelpText = "The amount of memory in KB to allocate.")]
        public ulong MemorySize { get; set; }

    }


}
