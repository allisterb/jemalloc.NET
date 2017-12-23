using System;
using System.Collections.Generic;
using System.Text.Utf8;

namespace jemalloc.Tests
{
    public struct TestRecord : IEquatable<TestRecord>
    {
        public Guid ID { get; set; }               //4+8 = 12
        public Utf8String FirstName { get; set; }      //16 hdr + 4 + (11*2) = 42
        public Utf8String LastName { get; set; }       //16 hdr + 4 + (18*2) = 56
        public DateTime? DOB { get; set; }         //10
        public decimal Balance { get; set; }       //8
        public FixedBuffer<char> Chars { get; }
        //public SafeArray<float> Data { get; set; }          //8
        //public SafeArray<byte> BinData { get; set; } //8

        public static TestRecord MakeTestRecord()
        {
            Guid _id = Guid.NewGuid();
            string _ids = _id.ToString("N");
            return new TestRecord()
            {
                ID = _id,
                FirstName = new Utf8String("Gavial-" + _id.ToString("0000")),
                LastName = new Utf8String("Buxarinovich-" + _id.ToString("00000")),
                DOB = _ids.StartsWith("7") ? (DateTime?)null : DateTime.UtcNow,
                Balance = 2131m
            };
        }

        public bool Equals(TestRecord r)
        {
            return this.ID == r.ID;
        }
    }
}
