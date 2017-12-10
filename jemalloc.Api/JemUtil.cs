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

        public static int SizeOfStruct<T>() where T : struct
        {
            return Unsafe.SizeOf<T>();
        }
    }
}
