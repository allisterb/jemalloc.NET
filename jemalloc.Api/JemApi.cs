using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Text;
using System.Threading;

namespace jemalloc
{
    #region Types and Enums
    public class CallerInformation
    {
        public string Name;
        public string File;
        public int LineNumber;

        public CallerInformation(string name, string file, int line_number)
        {
            this.Name = name;
            this.File = file;
            this.LineNumber = line_number;
        }

        public override string ToString() => Jem.GetCallerDetails(this);
        
    }

    internal enum ERRNO
    {
        ENONE = 0,
        EPERM = 1,
        ENOENT = 2,
        ESRCH = 3,
        EINTR = 4,
        EIO = 5,
        ENXIO = 6,
        E2BIG = 7,
        ENOEXEC = 8,
        EBADF = 9,
        ECHILD = 10,
        EAGAIN = 11,
        ENOMEM = 12,
        EACCES = 13,
        EFAULT = 14,
        EBUSY = 16,
        EEXIST = 17,
        EXDEV = 18,
        ENODEV = 19,
        ENOTDIR = 20,
        EISDIR = 21,
        ENFILE = 23,
        EMFILE = 24,
        ENOTTY = 25,
        EFBIG = 27,
        ENOSPC = 28,
        ESPIPE = 29,
        EROFS = 30,
        EMLINK = 31,
        EPIPE = 32,
        EDOM = 33,
        EDEADLK = 36,
        ENAMETOOLONG = 38,
        ENOLCK = 39,
        ENOSYS = 40,
        ENOTEMPTY = 41
    }

    [Flags]
    public enum ALLOC_FLAGS
    {
        ZERO,
        DETAILS
    }
    #endregion

    public unsafe static partial class Jem
    {
        #region Constructor
        static Jem()
        {
            __Internal.JeMallocMessage += messagesCallback;
        }
        #endregion

        #region Methods

        #region Low-level jemalloc API
        public static global::System.IntPtr Malloc(ulong size, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            CallerInformation caller = new CallerInformation(memberName, fileName, lineNumber);
            Initialized = true;
            IntPtr __ret = __Internal.JeMalloc(size);
            if (__ret != IntPtr.Zero)
            {
                if (!ImmutableInterlocked.TryAdd(ref _Allocations, __ret, 0))
                {
                    throw new Exception($"Could not add pointer {__ret} to Allocations ledger.");
                }
                return __ret;
            }
            else
            {
                throw new OutOfMemoryException($"Could not allocate {size} bytes for {GetCallerDetails(caller)}.");
            }
        }

        public static global::System.IntPtr Calloc(ulong num, ulong size, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            CallerInformation caller = new CallerInformation(memberName, fileName, lineNumber);
            Initialized = true;
            IntPtr __ret = __Internal.JeCalloc(num, size);
            if (__ret != IntPtr.Zero)
            {
                if (!ImmutableInterlocked.TryAdd(ref _Allocations, __ret, 0))
                {
                    throw new Exception($"Could not add pointer {__ret} to Allocationd ledger.");
                }
                return __ret;
            }
            else
            {
                throw new OutOfMemoryException($"Could not allocate {num * size} bytes for {GetCallerDetails(caller)}.");
            }
        }

        public static int PosixMemalign(void** memptr, ulong alignment, ulong size)
        {
            Initialized = true;
            var __ret = __Internal.JePosixMemalign(memptr, alignment, size);
            return __ret;
        }

        public static global::System.IntPtr AlignedAlloc(ulong alignment, ulong size)
        {
            Initialized = true;
            var __ret = __Internal.JeAlignedAlloc(alignment, size);
            return __ret;
        }

        public static global::System.IntPtr Realloc(global::System.IntPtr ptr, ulong size)
        {
            Initialized = true;
            var __ret = __Internal.JeRealloc(ptr, size);
            return __ret;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool Free(global::System.IntPtr ptr, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            bool ret = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {

            }
            finally
            {
                if (ImmutableInterlocked.TryRemove(ref _Allocations, ptr, out int refCount))
                {
                    __Internal.JeFree(ptr);
                        ret = true;
                }
            }
            
            return ret;
        }

        public static global::System.IntPtr Mallocx(ulong size, int flags)
        {
            Initialized = true;
            var __ret = __Internal.JeMallocx(size, flags);
            return __ret;
        }

        public static global::System.IntPtr Rallocx(global::System.IntPtr ptr, ulong size, int flags)
        {
            Initialized = true;
            var __ret = __Internal.JeRallocx(ptr, size, flags);
            return __ret;
        }

        public static ulong Xallocx(global::System.IntPtr ptr, ulong size, ulong extra, int flags)
        {
            Initialized = true;
            var __ret = __Internal.JeXallocx(ptr, size, extra, flags);
            return __ret;
        }

        public static ulong Sallocx(global::System.IntPtr ptr, int flags)
        {
            Initialized = true;
            var __ret = __Internal.JeSallocx(ptr, flags);
            return __ret;
        }

        public static void Dallocx(global::System.IntPtr ptr, int flags)
        {
            Initialized = true;
            __Internal.JeDallocx(ptr, flags);
        }

        public static void Sdallocx(global::System.IntPtr ptr, ulong size, int flags)
        {
            Initialized = true;
            __Internal.JeSdallocx(ptr, size, flags);
        }

        public static ulong Nallocx(ulong size, int flags)
        {
            Initialized = true;
            var __ret = __Internal.JeNallocx(size, flags);
            return __ret;
        }

        public static int Mallctl(string name, global::System.IntPtr oldp, ref ulong oldlenp, global::System.IntPtr newp, ulong newlen)
        {
            fixed (ulong* __refParamPtr2 = &oldlenp)
            {
                var __arg2 = __refParamPtr2;
                var __ret = __Internal.JeMallctl(name, oldp, __arg2, newp, newlen);
                return __ret;
            }
        }

        public static int Mallctlnametomib(string name, ref ulong mibp, ref ulong miblenp)
        {
            fixed (ulong* __refParamPtr1 = &mibp)
            {
                var __arg1 = __refParamPtr1;
                fixed (ulong* __refParamPtr2 = &miblenp)
                {
                    var __arg2 = __refParamPtr2;
                    var __ret = __Internal.JeMallctlnametomib(name, __arg1, __arg2);
                    return __ret;
                }
            }
        }

        public static int Mallctlbymib(ref ulong mib, ulong miblen, global::System.IntPtr oldp, ref ulong oldlenp, global::System.IntPtr newp, ulong newlen)
        {
            fixed (ulong* __refParamPtr0 = &mib)
            {
                var __arg0 = __refParamPtr0;
                fixed (ulong* __refParamPtr3 = &oldlenp)
                {
                    var __arg3 = __refParamPtr3;
                    var __ret = __Internal.JeMallctlbymib(__arg0, miblen, oldp, __arg3, newp, newlen);
                    return __ret;
                }
            }
        }
        #endregion

        #region High-level API
        public static bool Init(string conf)
        {
            if (!Initialized)
            {
                if (mallocMessagesBuilder == null)
                {
                    mallocMessagesBuilder = new StringBuilder();
                }
                try
                {
                    MallocConf = conf;
                }
                catch (Exception)
                {
                    throw new ArgumentException($"The configuration string \'{conf}\' is invalid");
                }
                Initialized = true;
                return true;
            }
            else return false;
        }

        public static int GetRefCount(IntPtr ptr)
        {
            return Allocations[ptr];
        }

        public static void IncrementRefCount(IntPtr ptr)
        {
            int refCount;
            do
            {
                refCount = Allocations[ptr];
            }
            while (!ImmutableInterlocked.TryUpdate(ref _Allocations, ptr, refCount + 1, refCount));
        }

        public static void DecrementRefCount(IntPtr ptr)
        {
            int refCount;
            do
            {
                refCount = Allocations[ptr];
            }
            while (!ImmutableInterlocked.TryUpdate(ref _Allocations, ptr, refCount - 1, refCount) && refCount > 0);
        }

        public static bool PtrIsAllocated(IntPtr ptr)
        {
            return Allocations.ContainsKey(ptr);
        }

        public static string MallocStats => GetMallocStats(string.Empty);

        public static string GetMallocStats(string opt)
        {
            StringBuilder statsBuilder = new StringBuilder(1000);
            __Internal.JeMallocMessageCallback stats = (o, m) => { statsBuilder.Append(m); };
            __Internal.JeMallocStatsPrint(Marshal.GetFunctionPointerForDelegate(stats), IntPtr.Zero, opt);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                statsBuilder = statsBuilder.Replace("\\n", "\\r\\n");
            }
            return statsBuilder.ToString();
        }

        public static ulong MallocUsableSize(global::System.IntPtr ptr)
        {
            var __ret = __Internal.JeMallocUsableSize(ptr);
            return __ret;
        }

        public static string MallocConf
        {
            get
            {
                return Marshal.PtrToStringAnsi(__Internal.JeGetMallocConf());
            }
            set
            {
                __Internal.JeSetMallocConf(Marshal.StringToHGlobalAnsi(value));
            }
        }

        #region MallCtl
        public static int GetMallCtlInt32(string name)
        {
            void* i = stackalloc int[1];
            ulong size = sizeof(Int32);
            ERRNO r = (ERRNO) Mallctl(name, (IntPtr) i, ref size, IntPtr.Zero, 0);
            return r == ERRNO.ENONE ? *(Int32*)(i) : throw GetExceptionForErrNo($"Could not get mallctl value {name}.", r);
        }

        public static bool SetMallCtlInt32(string name, int value)
        {
            void* i = &value;
            ulong size = sizeof(Int32);
            ERRNO r = (ERRNO) Mallctl(name, IntPtr.Zero, ref size, (IntPtr) i, size);
            return r == ERRNO.ENONE ? true : throw GetExceptionForErrNo($"Could not set mallctl value {name} to {value}.", r);
        }

        public static bool GetMallCtlBool(string name)
        {
            void* i = stackalloc byte[1];
            ulong size = sizeof(byte);
            ERRNO r = (ERRNO)Mallctl(name, (IntPtr)i, ref size, IntPtr.Zero, 0);
            return r == ERRNO.ENONE ? *(byte*)(i) == 1 ? true : false: 
                r == ERRNO.ENOENT ? false : throw GetExceptionForErrNo($"Could not get mallctl value {name}.", r);
         
        }

        public static bool SetMallCtlBool(string name, bool value)
        {
            byte v = value ? (byte) 1 : (byte) 0;
            void* n = &v;
            ulong size = sizeof(byte);
            ERRNO r = (ERRNO)Mallctl(name, IntPtr.Zero, ref size, (IntPtr)n, size);
            return r == ERRNO.ENONE ? true : throw GetExceptionForErrNo($"Could not set mallctl value {name} to {value}.", r);
        }

        public static UInt64 GetMallCtlUInt64(string name)
        {
            void* i = stackalloc UInt64[1];
            ulong size = sizeof(UInt64);
            ERRNO r = (ERRNO) Mallctl(name, (IntPtr) i, ref size, IntPtr.Zero, 0);
            return r == ERRNO.ENONE ? *(UInt64*)(i) : throw GetExceptionForErrNo($"Could not get mallctl value {name}.", r);
        }

        public static bool SetMallCtlUInt64(string name, ulong value)
        {
            void* n = &value;
            ulong size = sizeof(UInt64);
            ERRNO r = (ERRNO) Mallctl(name, IntPtr.Zero, ref size, (IntPtr) n, size);
            return r == ERRNO.ENONE ? true : throw GetExceptionForErrNo($"Could not set mallctl value {name} to {value}.", r);
        }

        public static Int64 GetMallCtlSInt64(string name)
        {
            void* i = stackalloc Int64[1];
            ulong size = sizeof(Int64);
            ERRNO r = (ERRNO) Mallctl(name, (IntPtr) i, ref size, IntPtr.Zero, 0);
            return r == ERRNO.ENONE ? *(Int64*)(i) : throw GetExceptionForErrNo($"Could not get mallctl value {name}.", r);
        }

        public static bool SetMallCtlSInt64(string name, long value)
        {
            void* n = &value;
            ulong size = sizeof(Int64);
            ERRNO r = (ERRNO) Mallctl(name, IntPtr.Zero, ref size, (IntPtr)n, size);
            return r == ERRNO.ENONE ? true : throw GetExceptionForErrNo($"Could not set mallctl value {name} to {value}.", r);
        }

        public static string GetMallCtlStr(string name)
        {
            void* p = stackalloc IntPtr[1];
            IntPtr retp = new IntPtr(p);
            ulong size = (ulong)sizeof(IntPtr);
            int ret = Mallctl(name, (IntPtr) p, ref size, IntPtr.Zero, 0);
            if ((ERRNO)ret == ERRNO.ENONE)
            {
                return Marshal.PtrToStringAnsi(*(IntPtr*)p);
            }
            else
            {
               throw GetExceptionForErrNo($"Could not get mallctl value {name}.", (ERRNO)ret);
            }
        }
        #endregion

        public static int TryFreeAll()
        {
            int c = Allocations.Count;
            foreach (IntPtr p in Allocations.Keys)
            {
                Jem.Free(p);
                ImmutableInterlocked.TryRemove(ref _Allocations, p, out int refCount);
            }
            
            return c;
        }

        public static Span<T> Malloc<T>(ulong size, int length, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) where T : struct
        {
            IntPtr ptr = Malloc(size, memberName, fileName, lineNumber);
            return new Span<T>((void*)ptr, length);
        }

        #region FixedBuffer
        public static IntPtr AllocateFixedBuffer<T>(ulong length, ulong size, long timestamp, int tid, int rid, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            IntPtr ptr = Jem.Calloc(length, size, memberName, fileName, lineNumber);
            if (ptr != IntPtr.Zero)
            {
                
                if (!ImmutableInterlocked.TryAdd(ref _FixedBufferAllocations, ptr, new FixedBufferAllocation(ptr, length * size, timestamp, tid, rid)))
                {
                    throw new Exception($"Could not add allocation record for ptr {ptr} to fixed buffer allocations ledger.");
                }
                /*
                ImmutableInterlocked.AddOrUpdate(ref _FixedBufferAllocations, ptr, (a) => new FixedBufferAllocation(ptr, length * size, timestamp, tid, rid),
                    (k, v) => new FixedBufferAllocation(ptr, length * size, timestamp, tid, rid));
                    */
            }
            return ptr;
        }

        public static bool FreeFixedBuffer(IntPtr ptr)
        {
            if (!ImmutableInterlocked.TryRemove(ref _FixedBufferAllocations, ptr, out FixedBufferAllocation a))
            {
                return false;
            }
            else if (!Jem.Free(ptr))
            {
                return false;
            }
            else return true;
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FixedBufferIsAllocatedWith(IntPtr ptr, ulong size, long timestamp, int tid, int rid)
        {
            if (!Allocations.ContainsKey(ptr))
            {
                return false;
            }
            else
            {
                FixedBufferAllocation a = new FixedBufferAllocation(ptr, size, timestamp, tid, rid);
                return FixedBufferAllocations.ContainsValue(a);
               
            }
        }
        #endregion

        #endregion

        #region Utility methods
        internal static bool AllocFlagSet(ALLOC_FLAGS flag)
        {
            return (flag & Flags) == flag;
        }

        internal static Exception GetExceptionForErrNo(string message, ERRNO no)
        {
            switch (no)
            {
                case ERRNO.ENOMEM:
                    return new OutOfMemoryException(message);
                default:
                    return new Exception(message + $" {no}.");
            }
        }

        internal static string GetCallerDetails(string memberName, string fileName, int lineNumber)
        {
            return $"Member {memberName} at line {lineNumber} in file {fileName}";
        }

        internal static string GetCallerDetails(CallerInformation c)
        {
            return $"Member {c.Name} at line {c.LineNumber} in file {c.File}";
        }
        #endregion

        #endregion

        #region Properties
        public static bool Initialized { get; private set; } = false;

        public static ALLOC_FLAGS Flags;

        #region Malloc Messages
        public static event JeMallocMessageAction MallocMessage;

        public static string MallocMessages => mallocMessagesBuilder.ToString();

        private static __Internal.JeMallocMessageCallback messagesCallback = (o, m) =>
        {
            mallocMessagesBuilder.Append(m);
            MallocMessage.Invoke(m);
        };
        #endregion

        #region jemalloc Statistics
        public static UInt64 AllocatedBytes => GetMallCtlUInt64("stats.allocated");
        public static UInt64 ActiveBytes => GetMallCtlUInt64("stats.active");
        public static UInt64 MappedBytes => GetMallCtlUInt64("stats.mapped");
        public static UInt64 ResidentBytes => GetMallCtlUInt64("stats.resident");

        #endregion

        #region Allocations ledgers
        public static ImmutableDictionary<IntPtr, int> Allocations => _Allocations;

        public static ImmutableDictionary<IntPtr, FixedBufferAllocation> FixedBufferAllocations => _FixedBufferAllocations;

        public static ConcurrentDictionary<IntPtr, int> FixedBufferRefCounts { get; private set; } = new ConcurrentDictionary<IntPtr, int>();

        public static List<Tuple<IntPtr, ulong, CallerInformation>> AllocationsDetails { get; private set; } = new List<Tuple<IntPtr, ulong, CallerInformation>>();
        #endregion
        #endregion

        #region Fields
        private static StringBuilder mallocMessagesBuilder = new StringBuilder();
        private static ImmutableDictionary<IntPtr, int> _Allocations = ImmutableDictionary.Create<IntPtr, int>();
        private static ImmutableDictionary<IntPtr, FixedBufferAllocation> _FixedBufferAllocations = ImmutableDictionary.Create<IntPtr, FixedBufferAllocation>();
        private static FixedBufferComparator fixedBufferComparator = new FixedBufferComparator();
        #endregion
    }
}
