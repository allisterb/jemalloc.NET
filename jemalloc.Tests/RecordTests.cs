using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class RecordTests : jemallocTest
    {
        [Fact(DisplayName = "Can create an array of TestRecords")]
        public void CanCreateTestRecordsArray()
        {
            SafeArray<TestRecord> records = new SafeArray<TestRecord>(100);
            for (int i = 0; i < records.Length; i++)
            {
                records[i] = TestRecord.MakeTestRecord(Rng);
            }
        }
    }
}
