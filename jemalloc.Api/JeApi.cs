using System;
using System.Runtime.InteropServices;
using System.Security;

namespace jemalloc
{
    public unsafe partial class Je
    {
        public static global::System.IntPtr Malloc(ulong size)
        {
            var __ret = __Internal.JeMalloc(size);
            return __ret;
        }

        public static global::System.IntPtr Calloc(ulong num, ulong size)
        {
            var __ret = __Internal.JeCalloc(num, size);
            return __ret;
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

        public static void Free(global::System.IntPtr ptr)
        {
            __Internal.JeFree(ptr);
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

        public static void MallocStatsPrint()
        {
            __Internal.JeMallocStatsPrint(IntPtr.Zero, IntPtr.Zero, string.Empty);
        }

        public static void MallocStatsPrint(string opt)
        {
            __Internal.JeMallocStatsPrint(IntPtr.Zero, IntPtr.Zero, opt);
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
            IntPtr retp = new IntPtr(i);
            ulong size = sizeof(Int32);
            Mallctl(name, retp, ref size, IntPtr.Zero, 0);
            return *(Int32*)(i);
        }

        public static bool GetMallCtlBool(string name)
        {
            return GetMallCtlInt32(name) == 1 ? true : false;
        }


        public static UInt64 GetMallCtlUInt64(string name)
        {
            void* i = stackalloc UInt64[1];
            IntPtr retp = new IntPtr(i);
            ulong size = sizeof(UInt64);
            Mallctl(name, retp, ref size, IntPtr.Zero, 0);
            return *(UInt64*)(i);
        }

        public static Int64 GetMallCtlSInt64(string name)
        {
            void* i = stackalloc Int64[1];
            IntPtr retp = new IntPtr(i);
            ulong size = sizeof(Int64);
            Mallctl(name, retp, ref size, IntPtr.Zero, 0);
            return *(Int64*)(i);
        }

        public static string GetMallCtlStr(string name)
        {
            IntPtr* p = stackalloc IntPtr[1];
            IntPtr retp = new IntPtr(p);
            ulong size = (ulong)sizeof(IntPtr);
            Mallctl(name, retp, ref size, IntPtr.Zero, 0);
            return Marshal.PtrToStringAnsi(*p);
        }

        public static bool Initialized { get; protected set; }

        public static event JeMallocMessageAction MallocMessage;

        public static bool Init(string conf)
        {
            if (Initialized)
            {
                return Initialized;
            }
            else
            {
                Je.MallocConf = conf;
                __Internal.JeMallocMessage = (o, m) => { MallocMessage.Invoke(m); };
                Initialized = true;
                return Initialized;
            }
        }

        internal enum ERR_NO
        {
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
    }
}
