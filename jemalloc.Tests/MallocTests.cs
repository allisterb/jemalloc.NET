using System;
using System.Diagnostics;
using Xunit;

namespace jemalloc.Tests
{
    public class MallocTests
    {
        public Process CurrentProcess { get; protected set; } = Process.GetCurrentProcess();

        public MallocTests() : base()
        {
            init_privateMemorySize = CurrentProcess.PrivateMemorySize64;
            init_peakPagedMem = CurrentProcess.PeakPagedMemorySize64;
            init_peakWorkingSet = CurrentProcess.PeakWorkingSet64;
            init_peakVirtualMem = CurrentProcess.PeakVirtualMemorySize64;
            init_allocated = Je.GetMallCtlUInt64("stats.allocated");
        }

        [Fact]
        public void CanMallocandFree()
        {
            long size = 100 * 1000 * 1000;
            Assert.True(init_privateMemorySize < size);
            Assert.True(init_allocated < (ulong) size);
            IntPtr p = Je.Malloc((ulong) size);
            string stats = Je.MallocStatsPrint();
            ulong allocated = Je.GetMallCtlUInt64("stats.allocated");
            CurrentProcess.Refresh();
            Assert.True((CurrentProcess.PrivateMemorySize64 - init_privateMemorySize) >= size);
            Assert.True(allocated > (ulong)size);
            Je.Free(p);
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
