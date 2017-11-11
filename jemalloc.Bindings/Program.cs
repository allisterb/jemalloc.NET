using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CppSharp;

namespace jemalloc.Bindings
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleDriver.Run(new JemallocLibrary());
        }
    }
}
