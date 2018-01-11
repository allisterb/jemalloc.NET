using System;
using System.Collections.Generic;
using System.Buffers;
using System.Text;

using jemalloc.Examples;

namespace jemalloc.Tests
{
    public class UDTTests : jemallocTest
    {
        public SafeArray<TestUDT> Employees;
        public UDTTests()
        {
            Employees = new SafeArray<TestUDT>(1024 * 1024);
            for (int i = 0; i < Employees.Length; i++)
            {
                Employees[i] = TestUDT.MakeTestRecord(JemUtil.Rng);
            }
        }

        public void CanVectorize()
        {
            Span<byte> s = Employees.GetSpan<byte>();
            
            int size = JemUtil.SizeOfStruct<TestUDT>();
            /*
            for (int i = 0; i < Employees.Length; i+= size * Employees[i].)
            {
               
                //s.Slice
                
            }
            */
        }

    }
}
