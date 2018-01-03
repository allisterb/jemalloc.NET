using System;
using System.Collections.Generic;


namespace jemalloc.Tests
{
    public struct TestRecord : IEquatable<TestRecord>
    {
        public Guid ID { get; set; }               //4+8 = 12
        public Utf8Buffer FirstName { get; set; }      //16 hdr + 4 + (11*2) = 42
        public Utf8Buffer LastName { get; set; }       //16 hdr + 4 + (18*2) = 56
        public DateTime? DOB { get; set; }         //10
        public decimal Balance { get; set; }       //8
        public FixedBuffer<float> Data { get; set; }          //8
        public FixedBuffer<byte> BinData { get; set; } //8

        public static TestRecord MakeTestRecord(Random rng)
        {
            Guid _id = Guid.NewGuid();
            string _ids = _id.ToString("N");
            byte[] binData = new byte[4096];
            rng.NextBytes(binData);
            return new TestRecord()
            {
                ID = _id,
                FirstName = new Utf8Buffer("Gavial-" + _id.ToString("D").Substring(0, 4)),
                LastName = new Utf8Buffer("Buxarinovich-" + _id.ToString("D").Substring(0, 4)),
                DOB = _ids.StartsWith("7") ? (DateTime?)null : DateTime.UtcNow,
                Balance = 2131m,
                BinData = new FixedBuffer<byte>(binData)
            };
        }

        public bool Equals(TestRecord r)
        {
            return this.ID == r.ID;
        }
    }
}
