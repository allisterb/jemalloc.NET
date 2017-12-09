using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Text;

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

        public override string ToString()
        {
            return Jem.GetCallerDetails(this);
        }
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
            if (!File.Exists(@".\jemallocd.dll"))
            {
                File.Copy(@"C:\Projects\jemalloc.NET\x64\Debug\jemallocd.dll", @".\jemallocd.dll");
            }
            __Internal.JeMallocMessage += messagesCallback;
        }
        #endregion

        #region Methods

        #region Low-level jemalloc API
        public static global::System.IntPtr Malloc(ulong size, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            CallerInformation caller = new CallerInformation(memberName, fileName, lineNumber);
            IntPtr __ret = __Internal.JeMalloc(size);
            if (__ret != IntPtr.Zero)
            {
                
                    Allocations.Add(__ret);
                    //AllocationsDetails.Add(new Tuple<IntPtr, ulong, CallerInformation>(__ret, size, caller));
                
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
            IntPtr __ret = __Internal.JeCalloc(num, size);
            if (__ret != IntPtr.Zero)
            {
                lock (allocationsLock)
                {
                    Allocations.Add(__ret);
                    //AllocationsDetails.Add(new Tuple<IntPtr, ulong, CallerInformation>(__ret, size, caller));
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
            var __ret = __Internal.JePosixMemalign(memptr, alignment, size);
            return __ret;
        }

        public static global::System.IntPtr AlignedAlloc(ulong alignment, ulong size)
        {
            var __ret = __Internal.JeAlignedAlloc(alignment, size);
            return __ret;
        }

        public static global::System.IntPtr Realloc(global::System.IntPtr ptr, ulong size)
        {
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
                if (Allocations.TryTake(out ptr))
                {
                    __Internal.JeFree(ptr);
                    ret = true;

                }
                else
                {
                    ret = false;
                }
            }
            return ret;
        }

        public static global::System.IntPtr Mallocx(ulong size, int flags)
        {
            var __ret = __Internal.JeMallocx(size, flags);
            return __ret;
        }

        public static global::System.IntPtr Rallocx(global::System.IntPtr ptr, ulong size, int flags)
        {
            var __ret = __Internal.JeRallocx(ptr, size, flags);
            return __ret;
        }

        public static ulong Xallocx(global::System.IntPtr ptr, ulong size, ulong extra, int flags)
        {
            var __ret = __Internal.JeXallocx(ptr, size, extra, flags);
            return __ret;
        }

        public static ulong Sallocx(global::System.IntPtr ptr, int flags)
        {
            var __ret = __Internal.JeSallocx(ptr, flags);
            return __ret;
        }

        public static void Dallocx(global::System.IntPtr ptr, int flags)
        {
            __Internal.JeDallocx(ptr, flags);
        }

        public static void Sdallocx(global::System.IntPtr ptr, ulong size, int flags)
        {
            __Internal.JeSdallocx(ptr, size, flags);
        }

        public static ulong Nallocx(ulong size, int flags)
        {
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
        public static void Init(string conf)
        {
            if (!Initialized)
            {
                MallocConf = conf;
                Initialized = true;
            }
        }

        public static void Init(ALLOC_FLAGS flags, string conf = null)
        {
            Flags = flags;
        }

        public static string MallocStatsPrint()
        {
            return MallocStatsPrint(string.Empty);
        }

        public static string MallocStatsPrint(string opt)
        {
            StringBuilder statsBuilder = new StringBuilder();
            __Internal.JeMallocMessageCallback stats = (o, m) => { statsBuilder.Append(m); };
            __Internal.JeMallocStatsPrint(Marshal.GetFunctionPointerForDelegate(stats), IntPtr.Zero, opt);
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

        public static int GetMallCtlInt32(string name)
        {
            void* i = stackalloc int[1];
            ulong size = sizeof(Int32);
            Mallctl(name, (IntPtr) i, ref size, IntPtr.Zero, 0);
            return *(Int32*)(i);
        }

        public static bool GetMallCtlBool(string name)
        {
            return GetMallCtlInt32(name) == 1 ? true : false;
        }


        public static UInt64 GetMallCtlUInt64(string name)
        {
            void* i = stackalloc UInt64[1];
            ulong size = sizeof(UInt64);
            Mallctl(name, (IntPtr) i, ref size, IntPtr.Zero, 0);
            return *(UInt64*)(i);
        }

        public static Int64 GetMallCtlSInt64(string name)
        {
            void* i = stackalloc Int64[1];
            ulong size = sizeof(Int64);
            Mallctl(name, (IntPtr) i, ref size, IntPtr.Zero, 0);
            return *(Int64*)(i);
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
               throw GetExceptionForErrNo((ERRNO) ret);
            }
        }

        public static int TryFreeAll()
        {
            int c = Allocations.Count;
            foreach (IntPtr p in Allocations)
            {
                Jem.Free(p);
            }
           
            return c;
        }

        public static Span<T> Malloc<T>(ulong size, int length, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) where T : struct
        {
            IntPtr ptr = Malloc(size, memberName, fileName, lineNumber);
            return new Span<T>((void*)ptr, length);
        }
        #endregion

            #region Utility Methods
            public static Span<T> GetSpanFromPtr<T>(IntPtr ptr, int length)
        {
            return new Span<T>((void*)ptr, length);
        }
        #endregion

        #region Utility methods
        internal static bool AllocFlagSet(ALLOC_FLAGS flag)
        {
            return (flag & Flags) == flag;
        }

        internal static Exception GetExceptionForErrNo(ERRNO no)
        {
            switch (no)
            {
                case ERRNO.ENOMEM:
                    return new OutOfMemoryException();
                default:
                    return new Exception();
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

        public static event JeMallocMessageAction MallocMessage;

        public static string MallocMessages => mallocMessagesBuilder.ToString();

        private static __Internal.JeMallocMessageCallback messagesCallback = (o, m) =>
        {
            mallocMessagesBuilder.Append(m);
            MallocMessage.Invoke(m);
        };

        public static ConcurrentBag<IntPtr> Allocations { get; private set; } = new ConcurrentBag<IntPtr>();

        public static List<Tuple<IntPtr, ulong, CallerInformation>> AllocationsDetails { get; private set; } = new List<Tuple<IntPtr, ulong, CallerInformation>>();
        
        public static bool PtrIsAllocated(IntPtr ptr)
        {
            lock (allocationsLock)
            {
                return Allocations.Any(a => a == ptr);
            }
        }
        #endregion

        #region Fields
        private static object allocationsLock = new object();
        private static StringBuilder mallocMessagesBuilder = new StringBuilder();
        #endregion
    }
}
