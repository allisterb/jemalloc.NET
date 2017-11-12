using System;
using System.Diagnostics;
using Xunit;

namespace jemalloc.Tests
{
    public class MallocTests
    {
        public Process CurrentProcess { get; protected set; } = Process.GetCurrentProcess();

        public MallocTests()
        {
            init_peakPagedMem = CurrentProcess.PeakPagedMemorySize64;
            init_peakWorkingSet = CurrentProcess.PeakWorkingSet64;
            init_peakVirtualMem = CurrentProcess.PeakVirtualMemorySize64;
        }

        [Fact]
        public void Test1()
        {
            IntPtr p = Je.Malloc(100 * 1000 * 1000);
            CurrentProcess.Refresh();
            Assert.True(CurrentProcess.PeakWorkingSet64 > init_peakWorkingSet);
        }

        #region Fields
        long init_peakPagedMem = 0;
        long init_peakWorkingSet = 0;
        long init_peakVirtualMem = 0;
        #endregion
    }
}
