using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace jemalloc
{
    public static class JemUtil
    {
        #region Constructor
        static JemUtil()
        {
            
        }
        #endregion

        #region Properties
        public static ConcurrentDictionary<string, object> BenchmarkValues { get; } = new ConcurrentDictionary<string, object>();

        public static ConcurrentDictionary<string, string> BenchmarkStatistics { get; } = new ConcurrentDictionary<string, string>();

        public static Process CurrentProcess { get; } =  Process.GetCurrentProcess();

        public static Random Rng { get; } = new Random();

        public static long ProcessPrivateMemory
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PrivateMemorySize64;
            }
        }
        public static long ProcessPeakVirtualMem
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PeakVirtualMemorySize64;
            }
        }

        public static long ProcessPeakWorkingSet
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PeakWorkingSet64;
            }
        }

        public static long ProcessPeakPagedMem
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PeakPagedMemorySize64;
            }
        }

        public static long ProcessVirtualMemory
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.VirtualMemorySize64;
            }
        }

        public static long ProcessWorkingSet
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.WorkingSet64;
            }
        }

        public static long ProcessPagedMemory
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PagedMemorySize64;
            }
        }

        public static Type CLRType<T>() => typeof(T);

        public static uint ElementSizeInBytes<T>() where T: struct => (uint)JemUtil.SizeOfStruct<T>();

        public static bool IsNumeric<T>() => JemUtil.IsNumericType<T>();

        public static int VectorLength<T>() where T : struct => IsNumeric<T>() ? Vector<T>.Count : 0;

        public static bool SIMD => Vector.IsHardwareAccelerated;
        #endregion

        #region Methods
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

        // System.String.GetHashCode(): http://referencesource.microsoft.com/#mscorlib/system/string.cs,0a17bbac4851d0d4
        // System.Web.Util.StringUtil.GetStringHashCode(System.String): http://referencesource.microsoft.com/#System.Web/Util/StringUtil.cs,c97063570b4e791a
        public static int CombineHashCodes(params int[] hashCodes)
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            int i = 0;
            foreach (var hashCode in hashCodes)
            {
                if (i % 2 == 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ hashCode;
                else
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ hashCode;

                ++i;
            }

            return hash1 + (hash2 * 1566083941);
        }

        public static string PrintBytes(double bytes, string suffix = "")
        {
            if (bytes >= 0 && bytes <= 1024)
            {
                return string.Format("{0:N0} B{1}", bytes, suffix);
            }
            else if (bytes >= 1024 && bytes < (1024 * 1024))
            {
                return string.Format("{0:N1} KB{1}", bytes / 1024, suffix);
            }
            else if (bytes >= (1024 * 1024) && bytes < (1024 * 1024 * 1024))
            {
                return string.Format("{0:N1} MB{1}", bytes / (1024 * 1024), suffix);
            }
            else if (bytes >= (1024 * 1024 * 1024) && (bytes < (1024f * 1024f * 1024f * 1024f)))
            {
                return string.Format("{0:N1} GB{1}", bytes / (1024 * 1024 * 1024), suffix);
            }
            else if (bytes >= (1024f * 1024f * 1024f * 1024f) && (bytes < (1024f * 1024f * 1024f * 1024f * 1024f)))
            {
                return string.Format("{0:N1} TB{1}", bytes / (1024f * 1024f * 1024f * 1024f), suffix);
            }
            else throw new ArgumentOutOfRangeException();

        }

        public static Tuple<double, string> PrintBytesToTuple(double bytes, string suffix = "")
        {
            string[] s = PrintBytes(bytes, suffix).Split(' ');
            return new Tuple<double, string>(Double.Parse(s[0]), s[1]);
        }

        public static ulong GetWindowsThreadCycles()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new PlatformNotSupportedException();
            }

            if (__Internal.QueryThreadCycleTime(__Internal.PseudoHandle, out ulong cycles))
            {
                return cycles;
            }
            else
            {
                throw new System.ComponentModel.Win32Exception();
            }
            
        }

        public partial struct __Internal
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool QueryThreadCycleTime(in IntPtr hThread, out ulong cycles);
            internal static readonly IntPtr PseudoHandle = (IntPtr)(-2);
        }
        #endregion

        #region Fields
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
        public static readonly Type CharCLRType = typeof(Char);
        public static readonly Type DecimalCLRType = typeof(Decimal);
        public static readonly Type IntPtrCLRType = typeof(IntPtr);
        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>(new Type[]
        {
            Int8CLRType, UInt8CLRType, Int16CLRType, UInt16CLRType, Int32CLRType, UInt32CLRType, Int64CLRType, UInt64CLRType,
            SingleCLRType, DoubleCLRType, CharCLRType, IntPtrCLRType
        });

        #endregion
    }
}
