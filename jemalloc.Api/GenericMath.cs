using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public static class GM<TData> where TData : struct, IEquatable<TData>, IComparable<TData>, IConvertible
    {
        #region Constructor
        static GM()
        {
            if (!JemUtil.IsNumericType<TData>())
            {
                throw new InvalidOperationException($"Type {typeof(TData).Name} is not a numeric type.");
            }

        }
        #endregion

        #region Properties
        public static Random Rng { get; } = new Random();
        #endregion

        #region Methods
        public static TData Const<TValue>(TValue v) where TValue : struct, IEquatable<TValue>, IComparable<TValue>, IConvertible
        {
            return (TData) Convert.ChangeType(v, typeof(TData));
        }

        public static TData Multiply(TData l, TData r) 
        {
            Tuple<TData, TData> value = new Tuple<TData, TData>(l, r);
            switch (value)
            {
                case Tuple<Byte, Byte> v:
                    return (TData)Convert.ChangeType(checked((byte)(v.Item1 * v.Item2)), typeof(TData));

                case Tuple<SByte, SByte> v:
                    return (TData)Convert.ChangeType(checked((SByte)(v.Item1 * v.Item2)), typeof(TData));

                case Tuple<UInt16, UInt16> v:
                    return (TData)Convert.ChangeType((checked((UInt16)(v.Item1 * v.Item2))), typeof(TData));

                case Tuple<Int16, Int16> v:
                    return (TData)Convert.ChangeType(checked((Int16)(v.Item1 * v.Item2)), typeof(TData));

                case Tuple<UInt32, UInt32> v:
                    return (TData)Convert.ChangeType(checked(v.Item1 * v.Item2), typeof(TData));

                case Tuple<Int32, Int32> v:
                    return (TData)Convert.ChangeType(checked(v.Item1 * v.Item2), typeof(TData));

                case Tuple<UInt64, UInt64> v:
                    return (TData)Convert.ChangeType(checked(v.Item1 * v.Item2), typeof(TData));

                case Tuple<Int64, Int64> v:
                    return (TData)Convert.ChangeType(checked(v.Item1 * v.Item2), typeof(TData));

                case Tuple<Single, Single> v:
                    return (TData)Convert.ChangeType(checked(v.Item1 * v.Item2), typeof(TData));

                case Tuple<Double, Double> v:
                    return (TData)Convert.ChangeType(checked(v.Item1 * v.Item2), typeof(TData));

                case Tuple<bool, bool> v:
                    throw new ArgumentException($"Cannot multiply 2 bools.");

                default:
                    throw new Exception($"Unsupported math type: {typeof(TData).Name}");
            }
        }
        
        public static double Sqrt(TData n)
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
                case bool v:
                    throw new ArgumentException($"Cannot get the square root of a bool.");
                default:
                    throw new ArithmeticException();
            }
        }

       
        public static double F(Func<double, double> f, TData n)
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
                case Boolean v:
                    throw new ArgumentException($"Cannot apply math functions to a bool.");
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
                    return Const(checked((sbyte)Rng.Next(0, SByte.MaxValue)));
                case Byte v:
                    return Const(checked((byte)Rng.Next(0, Byte.MaxValue)));
                case Int32 v:
                    return Const(checked((int)Rng.Next(0, Int32.MaxValue)));
                case UInt32 v:
                    return Const(checked((uint)Rng.Next(0, Int32.MaxValue)));
                case Int16 v:
                    return Const(checked((short)Rng.Next(0, Int16.MaxValue)));
                case UInt16 v:
                    return Const(checked((ushort)Rng.Next(0, UInt16.MaxValue)));
                case Int64 v:
                    return Const(checked((long)(Rng.NextDouble() * Int64.MaxValue)));
                case UInt64 v:
                    return Const(checked((ulong)(Rng.NextDouble() * UInt64.MaxValue)));
                case Single v:
                    return Const(checked(((Single)(Rng.NextDouble() * Int64.MaxValue))));
                case Double v:
                    return Const(checked((((double)Rng.NextDouble() * Int64.MaxValue))));
                case Boolean v:
                    return Const(Convert.ToBoolean(Rng.Next(0, 1)));

                default:
                    throw new ArithmeticException();
            }
        }

        public static TData Random(double max)
        {
            return Const(checked((double)Rng.NextDouble() * max));
        }

        public static (TData, TData) RandomMultiplyFactorAndValue()
        {
            TData e = default;
            TData max;
            int factor = Rng.Next(0, 4);
            switch (e)
            {
                case SByte v:
                    max = Random((sbyte)(sbyte.MaxValue / 4));
                    break;
                case Byte v:
                    max = Random((byte)(byte.MaxValue / (byte) 4));
                    break;
                case Int16 v:
                    max = Random((short)(short.MaxValue / (short)4));
                    break;
                case UInt16 v:
                    max = Random((ushort)(ushort.MaxValue / (ushort)4));
                    break;
                case Int32 v:
                    max = Random((int)(int.MaxValue / 4));
                    break;
                case UInt32 v:
                    max = Random(uint.MaxValue / 4u);
                    break;
                 case Int64 v:
                    max = Random((long)(long.MaxValue / 4));
                    break;
                case UInt64 v:
                    max = Random(ulong.MaxValue / 4u);
                    break;
                case Double v:
                    max = Random((double)(long.MaxValue / 4));
                    break;
                case Single v:
                    max = Random((Single)(long.MaxValue / 4));
                    break;
                default:
                    throw new ArgumentException($"Cannot multiply type {nameof(TData)}.");
            }
            return (Const(factor), Const(max));

        }
        #endregion

    }
}
