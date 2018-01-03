using System;
using System.Collections.Generic;


namespace jemalloc.Benchmarks
{
    public struct TestUDT : IEquatable<TestUDT>, IComparable<TestUDT>, IConvertible
    {
        public Guid ID { get; set; }               
        public Utf8Buffer FirstName { get; set; }      
        public Utf8Buffer LastName { get; set; }       
        public DateTime? DOB { get; set; }         
        public decimal Balance { get; set; }       
        public FixedBuffer<float> Data { get; set; }          
        public FixedBuffer<byte> Photo { get; set; } 

        public static TestUDT MakeTestRecord(Random rng)
        {
            Guid _id = Guid.NewGuid();
            string _ids = _id.ToString("N");
            byte[] photo = new byte[4096];
            rng.NextBytes(photo);
            return new TestUDT()
            {
                ID = _id,
                FirstName = new Utf8Buffer("Gavial-" + _id.ToString("D").Substring(0, 4)),
                LastName = new Utf8Buffer("Buxarinovich-" + _id.ToString("D").Substring(0, 4)),
                DOB = _ids.StartsWith("7") ? (DateTime?)null : DateTime.UtcNow,
                Balance = 2131m,
                Photo = new FixedBuffer<byte>(photo)
            };
        }

        public bool Equals(TestUDT r)
        {
            return this.ID == r.ID;
        }

        #region IConvertible


        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();        
        }

        double GetDoubleValue()
        {
            throw new InvalidCastException(); 
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return String.Format("({0}, {1})", FirstName, LastName);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }
        #endregion

        #region IComparable
        public int CompareTo(TestUDT other)
        {
            return 0;
        }
        #endregion
    }
}
