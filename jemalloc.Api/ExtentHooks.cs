using System;
using System.Runtime.InteropServices;
using System.Security;

namespace jemalloc
{
    public unsafe partial class ExtentHooks : IDisposable
    {
        [StructLayout(LayoutKind.Explicit, Size = 72)]
        public partial struct __Internal
        {
            [FieldOffset(0)]
            internal global::System.IntPtr alloc;

            [FieldOffset(8)]
            internal global::System.IntPtr dalloc;

            [FieldOffset(16)]
            internal global::System.IntPtr destroy;

            [FieldOffset(24)]
            internal global::System.IntPtr commit;

            [FieldOffset(32)]
            internal global::System.IntPtr decommit;

            [FieldOffset(40)]
            internal global::System.IntPtr purge_lazy;

            [FieldOffset(48)]
            internal global::System.IntPtr purge_forced;

            [FieldOffset(56)]
            internal global::System.IntPtr split;

            [FieldOffset(64)]
            internal global::System.IntPtr merge;

            [SuppressUnmanagedCodeSecurity]
            [DllImport("jemalloc", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint = "??0extent_hooks_s@@QEAA@AEBU0@@Z")]
            internal static extern global::System.IntPtr cctor(global::System.IntPtr instance, global::System.IntPtr _0);
        }

        public global::System.IntPtr __Instance { get; protected set; }

        protected int __PointerAdjustment;
        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::jemalloc.ExtentHooks> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::jemalloc.ExtentHooks>();
        protected void*[] __OriginalVTables;

        protected bool __ownsNativeInstance;

        internal static global::jemalloc.ExtentHooks __CreateInstance(global::System.IntPtr native, bool skipVTables = false)
        {
            return new global::jemalloc.ExtentHooks(native.ToPointer(), skipVTables);
        }

        internal static global::jemalloc.ExtentHooks __CreateInstance(global::jemalloc.ExtentHooks.__Internal native, bool skipVTables = false)
        {
            return new global::jemalloc.ExtentHooks(native, skipVTables);
        }

        private static void* __CopyValue(global::jemalloc.ExtentHooks.__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(global::jemalloc.ExtentHooks.__Internal));
            *(global::jemalloc.ExtentHooks.__Internal*)ret = native;
            return ret.ToPointer();
        }

        private ExtentHooks(global::jemalloc.ExtentHooks.__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected ExtentHooks(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new global::System.IntPtr(native);
        }

        public ExtentHooks()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::jemalloc.ExtentHooks.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public ExtentHooks(global::jemalloc.ExtentHooks _0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::jemalloc.ExtentHooks.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::jemalloc.ExtentHooks.__Internal*)__Instance) = *((global::jemalloc.ExtentHooks.__Internal*)_0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (__Instance == IntPtr.Zero)
                return;
            global::jemalloc.ExtentHooks __dummy;
            NativeToManagedMap.TryRemove(__Instance, out __dummy);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }
    }
}
