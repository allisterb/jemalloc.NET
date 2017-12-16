using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public static class GM<TData> where TData : struct, IEquatable<TData>
    {
        #region Constructor
        static GM()
        {
            if (!JemUtil.IsNumericType<TData>())
            {
                throw new ArithmeticException();
            }

        }
        #endregion

        #region Properties
        public static Random Rng { get; } = new Random();
        #endregion

        #region Methods
        public static TData Const<TValue>(TValue v) where TValue : struct, IEquatable<TValue>, IComparable<TValue>
        {
            return JemUtil.ValToGenericStruct<TValue, TData>((checked(v)));
        }

        public static TData Multiply(TData l, TData r) 
        {
            Tuple<TData, TData> value = new Tuple<TData, TData>(l, r);
            switch (value)
            {
                case Tuple<Byte, Byte> v:
                    return JemUtil.ValToGenericStruct<Byte, TData>(checked((byte)(v.Item1 * v.Item2)));

                case Tuple<SByte, SByte> v:
                    return JemUtil.ValToGenericStruct<SByte, TData>(checked((SByte)(v.Item1 * v.Item2)));

                case Tuple<UInt16, UInt16> v:
                    return JemUtil.ValToGenericStruct<UInt16, TData>(checked((UInt16)(v.Item1 * v.Item2)));

                case Tuple<Int16, Int16> v:
                    return JemUtil.ValToGenericStruct<Int16, TData>(checked((Int16)(v.Item1 * v.Item2)));

                case Tuple<UInt32, UInt32> v:
                    return JemUtil.ValToGenericStruct<UInt32, TData>(checked(v.Item1 * v.Item2));

                case Tuple<Int32, Int32> v:
                    return JemUtil.ValToGenericStruct<Int32, TData>(checked(v.Item1 * v.Item2));

                case Tuple<UInt64, UInt64> v:
                    return JemUtil.ValToGenericStruct<UInt64, TData>(checked(v.Item1 * v.Item2));

                case Tuple<Int64, Int64> v:
                    return JemUtil.ValToGenericStruct<Int64, TData>(checked(v.Item1 * v.Item2));

                case Tuple<Single, Single> v:
                    return JemUtil.ValToGenericStruct<Single, TData>(checked(v.Item1 * v.Item2));

                case Tuple<Double, Double> v:
                    return JemUtil.ValToGenericStruct<Double, TData>(checked(v.Item1 * v.Item2));
                default:
                    throw new Exception($"Unsupported type: {typeof(TData).Name}");
            }
        }
        
        public unsafe static double Sqrt(TData n)
        {
            switch (n)
            {
                case SByte v:
                    return checked(Math.Sqrt(v));
                case Byte v:
                    return checked(Math.Sqrt(v));
                case Int32 v:
                    return checked(Math.Sqrt(v));
                case UInt32 v:
                    return checked(Math.Sqrt(v));
                case Int16 v:
                    return checked(Math.Sqrt(v));
                case UInt16 v:
                    return checked(Math.Sqrt(v));
                case Int64 v:
                    return checked(Math.Sqrt(v));
                case UInt64 v:
                    return checked(Math.Sqrt(v));
                case Single v:
                    return checked(Math.Sqrt(v));
                case Double v:
                    return checked(Math.Sqrt(v));
                default:
                    throw new ArithmeticException();
            }
        }

        public unsafe static double F(Func<double, double> f, TData n)
        {
            switch (n)
            {
                case SByte v:
                    return checked(f(v));
                case Byte v:
                    return checked(f(v));
                case Int32 v:
                    return checked(f(v));
                case UInt32 v:
                    return checked(f(v));
                case Int16 v:
                    return checked(f(v));
                case UInt16 v:
                    return checked(f(v));
                case Int64 v:
                    return checked(f(v));
                case UInt64 v:
                    return checked(f(v));
                case Single v:
                    return checked(f(v));
                case Double v:
                    return checked(f(v));
                default:
                    throw new ArithmeticException();
            }
        }

        public static TData Random()
        {
            TData e = default;
            switch (e)
            {
                case SByte v:
                    return JemUtil.ValToGenericStruct<SByte, TData>(checked((sbyte)Rng.Next(0, SByte.MaxValue)));
                case Byte v:
                    return JemUtil.ValToGenericStruct<Byte, TData>(checked((byte)Rng.Next(0, Byte.MaxValue)));
                case Int32 v:
                    return JemUtil.ValToGenericStruct<Int32, TData>(checked((int)Rng.Next(0, Int32.MaxValue)));
                case UInt32 v:
                    return JemUtil.ValToGenericStruct<UInt32, TData>(checked((uint)Rng.Next(0, Int32.MaxValue)));
                case Int16 v:
                    return JemUtil.ValToGenericStruct<Int16, TData>(checked((short)Rng.Next(0, Int16.MaxValue)));
                case UInt16 v:
                    return JemUtil.ValToGenericStruct<UInt16, TData>(checked((ushort)Rng.Next(0, UInt16.MaxValue)));
                case Int64 v:
                    return JemUtil.ValToGenericStruct<Int64, TData>(checked((long)Rng.NextDouble() * Int64.MaxValue));
                case UInt64 v:
                    return JemUtil.ValToGenericStruct<UInt64, TData>(checked((ulong)Rng.NextDouble() * UInt64.MaxValue));
                default:
                    throw new ArithmeticException();
            }
        }

        public static Tuple<TData, TData> RandomMultiplyFactorAndValue()
        {
            TData e = default;
            TData max;
            int factor = Rng.Next(1, 4);
            switch (e)
            {
                case SByte v:
                    max = JemUtil.ValToGenericStruct<sbyte, TData>( (sbyte.MaxValue / (sbyte) 4));
                    break;
                case Byte v:
                    max = JemUtil.ValToGenericStruct<byte, TData>((byte)(byte.MaxValue / (byte) 4));
                    break;
                case Int32 v:
                    max = JemUtil.ValToGenericStruct<int, TData>((int)(int.MaxValue / 4));
                    break;
                case UInt32 v:
                    max = JemUtil.ValToGenericStruct<uint, TData>(uint.MaxValue / 4u);
                    break;
                case Int16 v:
                    max = JemUtil.ValToGenericStruct<short, TData>((short)(short.MaxValue / (short) 4));
                    break;
                case UInt16 v:
                    max = JemUtil.ValToGenericStruct<ushort, TData>((ushort)(ushort.MaxValue / (ushort) 4));
                    break;
                case Int64 v:
                    max = JemUtil.ValToGenericStruct<long, TData>((long)(long.MaxValue / 4));
                    break;
                case UInt64 v:
                    max = JemUtil.ValToGenericStruct<ulong, TData>(ulong.MaxValue / 4u);
                    break;
                case Double v:
                    max = JemUtil.ValToGenericStruct<double, TData>((double)(long.MaxValue / 4));
                    break;
                case Single v:
                    max = JemUtil.ValToGenericStruct<Single, TData>((Single)(long.MaxValue / 4));
                    break;
                default:
                    throw new ArithmeticException();
            }
            return new Tuple<TData, TData>(JemUtil.ValToGenericStruct<int, TData>(factor), JemUtil.ValToGenericStruct<int, TData>(20));

        }
        #endregion

    }
}
