using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace jemalloc.Benchmarks
{
    [OrderProvider(methodOrderPolicy: MethodOrderPolicy.Declared)]
    public class MallocVsArrayBenchmark<T> : JemBenchmark<T, int> where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public int ArraySize => Parameter;
        public const int InitialLargeBlockSize = 64 * 1024 * 1024; //64 MB
        public const int SmallBlockSize = 900000;
        public int LoopCount => Parameter;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            base.GlobalSetup();

            if (!Jem.SetMallCtlSInt64("arenas.dirty_decay_ms", 10) || !Jem.SetMallCtlSInt64("arenas.muzzy_decay_ms", 10) || !Jem.SetMallCtlBool("opt.background_thread", true))
            {
                throw new Exception();

            }
        }



        #region Create
        [Benchmark(Description = "Create array of data on the managed heap.")]
        [BenchmarkCategory("Create")]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            someData = null;
         }

        [Benchmark(Description = "Malloc buffer and Span<T> on the system managed heap.")]
        [BenchmarkCategory("Create")]
        public void CreateSpan()
        {
            ulong msize = (ulong)(ArraySize * JemUtil.SizeOfStruct<T>());
            IntPtr ptr = Jem.Malloc(msize);
            unsafe
            {
                Span<T> s = new Span<T>(ptr.ToPointer(), ArraySize);
            }
            Jem.Free(ptr);
        }
        #endregion

        #region Fill
        [Benchmark(Description = "Fill array of data on the managed heap.")]
        [BenchmarkCategory("Fill")]
        public void FillManagedArray()
        {
            T fill = GM<T>.Random();
            T[] someData = new T[ArraySize];
            for (int i = 0; i < someData.Length; i++)
            {
                someData[i] = fill;
            }
            T r = someData[ArraySize / 2];
        }

        [Benchmark(Description = "Fill memory on the system unmanaged heap using Span<T> with a single value.")]
        [BenchmarkCategory("Fill")]
        public void FillSpan()
        {
            T fill = GM<T>.Random();
            ulong msize = (ulong)(ArraySize * JemUtil.SizeOfStruct<T>());
            IntPtr ptr = Jem.Malloc(msize);
            Span<T> s = JemUtil.PtrToSpan<T>(ptr, ArraySize);
            s.Fill(fill);
            T r = s[ArraySize / 2];
            Jem.Free(ptr);
        }
        #endregion

        #region Fragment
        
        [Benchmark(Description = "Run an allocation pattern that won't fragment the Large Object Heap.", Baseline = true)]
        [BenchmarkCategory("Fragment")]
        public void FragmentLOHBaseline()
        {
            int largeBlockSize = InitialLargeBlockSize;
            int i = 0;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    largeBlockSize = largeBlockSize + (4 * 1024 * 1024);
                    if ((i + 1) % 50 == 0 && i != 0)
                    {
                        Info("Block size is {0} bytes.", PrintBytes(largeBlockSize));
                    }
                    T[] bigBlock = new T[largeBlockSize];
                    T[] smallBlock = new T[SmallBlockSize];
                }
            }
            catch (OutOfMemoryException)
            {
                Error($"OOM at index {i}.");
                return;
            }
            finally
            {
                GC.Collect();
            }
        }

        [Benchmark(Description = "Run an allocation pattern that fragments the Large Object Heap.")]
        [BenchmarkCategory("Fragment")]
        public void FragmentLOH()
        {
            int largeBlockSize = InitialLargeBlockSize;
            List<T[]> smallBlocks = new List<T[]>();
            int i = 0;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    largeBlockSize = largeBlockSize + (4 * 1024 * 1024);
                    if ((i + 1) % 50 == 0 && i != 0)
                    {
                        Info("Block size is {0} bytes.", PrintBytes(largeBlockSize));
                    }
                    T[] bigBlock = new T[largeBlockSize];
                    T[] smallBlock = new T[SmallBlockSize];
                    smallBlocks.Add(smallBlock);                    
                }
            }
            catch (OutOfMemoryException)
            {
                Error($"OOM at index {i}.");
                return;
            }
            finally
            {
                smallBlocks = null;
                GC.Collect();
            }
        }

        [Benchmark(Description = "Run an allocation pattern that fragments the Large Object Heap with regular GC compaction.")]
        [BenchmarkCategory("Fragment")]
        public void FragmentLOHWithCompact()
        {
            int largeBlockSize = InitialLargeBlockSize;
            List<T[]> smallBlocks = new List<T[]>();
            int i = 0;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    largeBlockSize = largeBlockSize + (4 * 1024 * 1024);
                    if ((i + 1) % 50 == 0 && i != 0)
                    {
                        Info("Block size is {0} bytes.", PrintBytes(largeBlockSize));
                    }
                    if (i % 10 == 0 && i != 0)
                    {
                        Info("Compacting LOH.");
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                    }
                    T[] bigBlock = new T[largeBlockSize];
                    T[] smallBlock = new T[SmallBlockSize];
                    smallBlocks.Add(smallBlock);

                }
            }
            catch (OutOfMemoryException)
            {
                Error($"OOM at index {i}.");
                return; 
            }
            finally
            {
                smallBlocks = null;
                GC.Collect();
            }
        }
        
        [Benchmark(Description = "Run an allocation pattern that fragments the unmanaged heap.")]
        [BenchmarkCategory("Fragment")]
        public void FragmentNativeHeap()
        {

            Info($"Dirty decay time: {Jem.GetMallCtlSInt64("arenas.dirty_decay_ms")} ms");
            int largeBlockSize = InitialLargeBlockSize;
            SafeArray<FixedBuffer<T>> smallBlocks = new SafeArray<FixedBuffer<T>>(LoopCount);
            int i = 0;
            FixedBuffer<T> bigBlock = default;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    largeBlockSize = largeBlockSize + (4 * 1024 * 1024);
                     
                    if ((i + 1) % 50 == 0 && i != 0)
                    {
                        Info("Block size is {0} bytes.", PrintBytes(largeBlockSize));
                    }
                    bigBlock = new FixedBuffer<T>(largeBlockSize);
                    FixedBuffer<T> smallBlock = new FixedBuffer<T>(SmallBlockSize);
                    smallBlocks[i] = smallBlock;
                    if (!bigBlock.Free()) throw new Exception();
                }
            }
            catch (OutOfMemoryException)
            {
                Info(Jem.MallocStats);
                Error($"Out-of-Memory at index {i} with large block size {largeBlockSize}.");
                return;
            }
            finally
            {
                Jem.TryFreeAll();
            }
        }

        #endregion
    }
}
