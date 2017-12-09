using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace jemalloc
{
    #region Delegates
    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public unsafe delegate global::System.IntPtr ExtentAllocT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, ulong _3, bool* _4, bool* _5, uint _6);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public unsafe delegate bool ExtentDallocT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, [MarshalAs(UnmanagedType.I1)] bool _3, uint _4);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public unsafe delegate void ExtentDestroyT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, [MarshalAs(UnmanagedType.I1)] bool _3, uint _4);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public unsafe delegate bool ExtentCommitT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, ulong _3, ulong _4, uint _5);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public unsafe delegate bool ExtentDecommitT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, ulong _3, ulong _4, uint _5);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public unsafe delegate bool ExtentPurgeT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, ulong _3, ulong _4, uint _5);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public unsafe delegate bool ExtentSplitT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, ulong _3, ulong _4, [MarshalAs(UnmanagedType.I1)] bool _5, uint _6);

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public unsafe delegate bool ExtentMergeT(global::System.IntPtr _0, global::System.IntPtr _1, ulong _2, global::System.IntPtr _3, ulong _4, [MarshalAs(UnmanagedType.I1)] bool _5, uint _6);

    public delegate void JeMallocMessageAction(string m);
    #endregion

    public unsafe static partial class Jem
    {
        public partial struct __Internal
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_malloc")]
            internal static extern global::System.IntPtr JeMalloc([In] ulong size);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_calloc")]
            internal static extern global::System.IntPtr JeCalloc([In] ulong num, [In] ulong size);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_posix_memalign")]
            internal static extern int JePosixMemalign(void** memptr, ulong alignment, ulong size);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_aligned_alloc")]
            internal static extern global::System.IntPtr JeAlignedAlloc(ulong alignment, ulong size);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_realloc")]
            internal static extern global::System.IntPtr JeRealloc(global::System.IntPtr ptr, ulong size);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_free")]
            internal static extern void JeFree(global::System.IntPtr ptr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_mallocx")]
            internal static extern global::System.IntPtr JeMallocx(ulong size, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_rallocx")]
            internal static extern global::System.IntPtr JeRallocx(global::System.IntPtr ptr, ulong size, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_xallocx")]
            internal static extern ulong JeXallocx(global::System.IntPtr ptr, ulong size, ulong extra, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_sallocx")]
            internal static extern ulong JeSallocx(global::System.IntPtr ptr, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_dallocx")]
            internal static extern void JeDallocx(global::System.IntPtr ptr, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_sdallocx")]
            internal static extern void JeSdallocx(global::System.IntPtr ptr, ulong size, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_nallocx")]
            internal static extern ulong JeNallocx(ulong size, int flags);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_mallctl")]
            internal static extern int JeMallctl([MarshalAs(UnmanagedType.LPStr)] [In] string name, global::System.IntPtr oldp, ulong* oldlenp, global::System.IntPtr newp, ulong newlen);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_mallctlnametomib")]
            internal static extern int JeMallctlnametomib([MarshalAs(UnmanagedType.LPStr)] string name, ulong* mibp, ulong* miblenp);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_mallctlbymib")]
            internal static extern int JeMallctlbymib(ulong* mib, ulong miblen, global::System.IntPtr oldp, ulong* oldlenp, global::System.IntPtr newp, ulong newlen);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_malloc_stats_print")]
            internal static extern void JeMallocStatsPrint(global::System.IntPtr write_cb, global::System.IntPtr je_cbopaque, [MarshalAs(UnmanagedType.LPStr)] string opts);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint="je_malloc_usable_size")]
            internal static extern ulong JeMallocUsableSize(global::System.IntPtr ptr);

            [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(global::System.Runtime.InteropServices.CallingConvention.Cdecl)]
            internal unsafe delegate void JeMallocMessageCallback(global::System.IntPtr _0, [MarshalAs(UnmanagedType.LPStr)] string _1);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl, 
                EntryPoint = "je_set_malloc_conf", CharSet = CharSet.Ansi)]
            internal static extern void JeSetMallocConf(IntPtr ptr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl, 
                EntryPoint = "je_get_malloc_conf", CharSet = CharSet.Ansi)]
            internal static extern IntPtr JeGetMallocConf();

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "je_get_malloc_message_ptr")]
            internal static extern global::System.IntPtr JeGetMallocMessagePtr();

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemallocd", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "je_set_malloc_message_ptr")]
            internal static extern void JeSetMallocMessagePtr(global::System.IntPtr p);

            internal static JeMallocMessageCallback JeMallocMessage
            {
                get
                {
                    var ret = __Internal.JeGetMallocMessagePtr();
                    return ret == IntPtr.Zero ? null :
                        (__Internal.JeMallocMessageCallback)Marshal.GetDelegateForFunctionPointer(ret, typeof(__Internal.JeMallocMessageCallback));
                }

                set
                {
                    IntPtr ptr = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
                    __Internal.JeSetMallocMessagePtr(ptr);
                }
            }
        }


    }   
}
