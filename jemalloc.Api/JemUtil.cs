using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace jemalloc
{
    public static class JemUtil
    {
        public static readonly Type Int8CLRType = typeof(SByte);
        public static readonly Type UInt8CLRType = typeof(Byte);
        public static readonly Type Int16CLRType = typeof(Int16);
        public static readonly Type UInt16CLRType = typeof(UInt16);
        public static readonly Type Int32CLRType = typeof(Int32);
        public static readonly Type UInt32CLRType = typeof(UInt32);
        public static readonly Type Int64CLRType = typeof(Int64);
        public static readonly Type UInt64CLRType = typeof(UInt64);
        public static readonly Type SingleCLRType = typeof(Single);
        public static readonly Type DoubleCLRType = typeof(Double);
        public static readonly Type StringCLRType = typeof(String);
        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>(new Type[]
        {
            Int8CLRType, UInt8CLRType, Int16CLRType, UInt16CLRType, Int32CLRType, UInt32CLRType, Int64CLRType, UInt64CLRType,
            SingleCLRType, DoubleCLRType
        });

        public static bool IsNumericType<T>()
        {
            return NumericTypes.Contains(typeof(T));
        }

        public unsafe static Span<T> PtrToSpan<T>(IntPtr ptr, int length)
        {
            return new Span<T>((void*)ptr, length);
        }


        public unsafe static ref readonly T PtrToStruct<T>(void* ptr) where T : struct
        {
            return ref Unsafe.AsRef<T>(ptr);
        }

        public unsafe static TReturn ValToGenericStruct<TValue, TReturn>(TValue v) where TValue : struct where TReturn : struct
        {
            void* ptr = Unsafe.AsPointer(ref v);
            return PtrToStruct<TReturn>(ptr);
        }

        public unsafe static TReturn GenericMultiply<TReturn>(TReturn l, TReturn r) where TReturn : struct
        {
            Tuple<TReturn, TReturn> value = new Tuple<TReturn, TReturn>(l, r);
            switch (value)
            {
                case Tuple<Byte, Byte> v:
                    return JemUtil.ValToGenericStruct<Byte, TReturn>((byte) (v.Item1 * v.Item2));

                case Tuple<SByte, SByte> v:
                    return JemUtil.ValToGenericStruct<SByte, TReturn>((SByte) (v.Item1 * v.Item2));

                case Tuple<UInt16, UInt16> v:
                    return JemUtil.ValToGenericStruct<UInt16, TReturn>((UInt16) (v.Item1 * v.Item2));

                case Tuple<Int16, Int16> v:
                    return JemUtil.ValToGenericStruct<Int16, TReturn>((Int16) (v.Item1 * v.Item2));

                case Tuple<UInt32, UInt32> v:
                    return JemUtil.ValToGenericStruct<UInt32, TReturn>((UInt32) (v.Item1 * v.Item2));

                case Tuple<Int32, Int32> v:
                    return JemUtil.ValToGenericStruct<Int32, TReturn>((Int32)(v.Item1 * v.Item2));

                case Tuple<UInt64, UInt64> v:
                    return JemUtil.ValToGenericStruct<UInt64, TReturn>((UInt64)(v.Item1 * v.Item2));

                case Tuple<Int64, Int64> v:
                    return JemUtil.ValToGenericStruct<Int64, TReturn>((Int64)(v.Item1 * v.Item2));

                default:
                    return default;
            }
        }

        public unsafe static double GenericSqrt<TReturn>(TReturn l) where TReturn : struct
        {
            if (!JemUtil.IsNumericType<TReturn>())
            {
                throw new ArithmeticException();
            }
            switch (l)
            {
                case SByte v:
                    return Math.Sqrt(v);
                case Byte v:
                    return Math.Sqrt(v);
                case Int32 v:
                    return Math.Sqrt(v);
                case UInt32 v:
                    return Math.Sqrt(v);
                case Int16 v:
                    return Math.Sqrt(v);
                case UInt16 v:
                    return Math.Sqrt(v);
                case Int64 v:
                    return Math.Sqrt(v);
                case UInt64 v:
                    return Math.Sqrt(v);
                default:
                    throw new ArithmeticException();
            }
         }


        public static int SizeOfStruct<T>() where T : struct
        {
            return Unsafe.SizeOf<T>();
        }
    }
}
