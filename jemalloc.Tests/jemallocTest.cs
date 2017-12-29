using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public abstract class jemallocTest
    {
        public static Random Rng = new Random();
        public Process CurrentProcess { get; protected set; } = Process.GetCurrentProcess();

        public jemallocTest()
        {
            Jem.Init("dirty_decay_ms:1,muzzy_decay_ms:1,tcache:false,narenas:3");
            init_privateMemorySize = CurrentProcess.PrivateMemorySize64;
            init_peakPagedMem = CurrentProcess.PeakPagedMemorySize64;
            init_peakWorkingSet = CurrentProcess.PeakWorkingSet64;
            init_peakVirtualMem = CurrentProcess.PeakVirtualMemorySize64;
            init_allocated = Jem.AllocatedBytes;
        }

        #region Fields
        long init_privateMemorySize = 0;
        long init_peakPagedMem = 0;
        long init_peakWorkingSet = 0;
        long init_peakVirtualMem = 0;
        ulong init_allocated;
        #endregion
    }
}
