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
        [Option('s', "size", Required = true, HelpText = "The Azure Storage resource or local directory that will be the sync source. For an Azure Storage resource you must use your Blob Service endpoint Uri.")]
        public long MemorySize { get; set; }

    }


}
