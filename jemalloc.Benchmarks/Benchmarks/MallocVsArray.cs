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
        public const int SmallBlockSize = 90000;
        public int LoopCount => Parameter;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            base.GlobalSetup();
            Jem.Init("dirty_decay_ms:0,muzzy_decay_ms:0,junk:false,tcache:false");
        }

        #region Create
        [Benchmark(Description = "Create array of data on the managed heap.")]
        [BenchmarkCategory("Create")]
        public void CreateManagedArray()
        {
            T[] someData = new T[ArraySize];
            SetMemoryStatistics();
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
            SetMemoryStatistics();
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
        [BenchmarkCategory("Managed", "Fragment")]
        public void FragmentLOHBaseline()
        {
            int largeBlockSize = InitialLargeBlockSize;
            int i = 0;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    T[] bigBlock = new T[largeBlockSize];
                    T[] smallBlock = new T[SmallBlockSize];
                    largeBlockSize = largeBlockSize + (1 * 1024 * 1024);
                }
                this.SetStatistic($"{nameof(FragmentLOHBaseline)}_WorkingSet", JemUtil.PrintBytes(JemUtil.ProcessWorkingSet));
                this.SetStatistic($"{nameof(FragmentLOHBaseline)}_JemResident", JemUtil.PrintBytes(Jem.ResidentBytes));
                this.SetStatistic($"{nameof(FragmentLOHBaseline)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
                this.SetStatistic($"{nameof(FragmentLOHBaseline)}_JemAllocated", JemUtil.PrintBytes(Jem.AllocatedBytes));
            }
            catch (OutOfMemoryException)
            {
                Error($"OOM at index {i}.");
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        [Benchmark(Description = "Run an allocation pattern that fragments the Large Object Heap.")]
        [BenchmarkCategory("Managed", "Fragment")]
        public void FragmentLOH()
        {
            int largeBlockSize = InitialLargeBlockSize;
            List<T[]> smallBlocks = new List<T[]>();
            int i = 0;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    T[] bigBlock = new T[largeBlockSize];
                    T[] smallBlock = new T[SmallBlockSize];
                    smallBlocks.Add(smallBlock);
                    largeBlockSize = largeBlockSize + (1 * 1024 * 1024);

                }
                this.SetStatistic($"{nameof(FragmentLOH)}_WorkingSet", JemUtil.PrintBytes(JemUtil.ProcessWorkingSet));
                this.SetStatistic($"{nameof(FragmentLOH)}_JemResident", JemUtil.PrintBytes(Jem.ResidentBytes));
                this.SetStatistic($"{nameof(FragmentLOH)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
                this.SetStatistic($"{nameof(FragmentLOH)}_JemAllocated", JemUtil.PrintBytes(Jem.AllocatedBytes));

            }
            catch (OutOfMemoryException)
            {
                Error($"OOM at index {i}.");
                throw;
            }
            finally
            {
                smallBlocks = null;
                GC.Collect();
            }
        }

        [Benchmark(Description = "Run an allocation pattern that fragments the Large Object Heap with regular GC compaction.")]
        [BenchmarkCategory("Managed", "Fragment")]
        public void FragmentLOHWithCompact()
        {
            int largeBlockSize = InitialLargeBlockSize;
            List<T[]> smallBlocks = new List<T[]>();
            int i = 0;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    if ((i + 1)% 10 == 0)
                    {
                        //Info("Compacting LOH.");
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                    }
                    T[] bigBlock = new T[largeBlockSize];
                    T[] smallBlock = new T[SmallBlockSize];
                    smallBlocks.Add(smallBlock);
                    largeBlockSize = largeBlockSize + (1 * 1024 * 1024);
                }
                this.SetStatistic($"{nameof(FragmentLOHWithCompact)}_WorkingSet", JemUtil.PrintBytes(JemUtil.ProcessWorkingSet));
                this.SetStatistic($"{nameof(FragmentLOHWithCompact)}_JemResident", JemUtil.PrintBytes(Jem.ResidentBytes));
                this.SetStatistic($"{nameof(FragmentLOHWithCompact)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
                this.SetStatistic($"{nameof(FragmentLOHWithCompact)}_JemAllocated", JemUtil.PrintBytes(Jem.AllocatedBytes));

            }
            catch (OutOfMemoryException)
            {
                Error($"OOM at index {i}.");
                throw; 
            }
            finally
            {
                smallBlocks = null;
                GC.Collect();
            }
        }

        [Benchmark(Description = "Run an allocation pattern that won't fragment the unmanaged heap.")]
        [BenchmarkCategory("Unmanaged", "Fragment")]
        public void FragmentNativeHeapBaseline()
        {

            Info($"Dirty decay time: {Jem.GetMallCtlSInt64("arenas.dirty_decay_ms")} ms");
            int largeBlockSize = InitialLargeBlockSize;
            int i = 0;
            FixedBuffer<T> bigBlock = default;
            try
            {
                for (i = 0; i < LoopCount; i++)
                {
                    bigBlock = new FixedBuffer<T>(largeBlockSize);
                    FixedBuffer<T> smallBlock = new FixedBuffer<T>(SmallBlockSize);
                    int j = JemUtil.Rng.Next(0, ArraySize);
                    T r = GM<T>.Random();
                    smallBlock[j] = r;
                    if (!smallBlock[j].Equals(r))
                    {
                        throw new Exception($"Cannot validate small block at index {i}.");
                    }
                    if (!smallBlock.Free()) throw new Exception("Cannot free smallBlock.");
                    if (!bigBlock.Free()) throw new Exception("Cannot free bigBlock.");
                    largeBlockSize = largeBlockSize + (1 * 1024 * 1024);
                }
                this.SetStatistic($"{nameof(FragmentNativeHeapBaseline)}_WorkingSet", JemUtil.PrintBytes(JemUtil.ProcessWorkingSet));
                this.SetStatistic($"{nameof(FragmentNativeHeapBaseline)}_JemResident", JemUtil.PrintBytes(Jem.ResidentBytes));
                this.SetStatistic($"{nameof(FragmentNativeHeapBaseline)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
                this.SetStatistic($"{nameof(FragmentNativeHeapBaseline)}_JemAllocated", JemUtil.PrintBytes(Jem.AllocatedBytes));

            }
            catch (OutOfMemoryException)
            {
                Info(Jem.MallocStats);
                Error($"Out-of-Memory at index {i} with large block size {largeBlockSize}.");
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        [Benchmark(Description = "Run an allocation pattern that fragments the unmanaged heap.")]
        [BenchmarkCategory("Unmanaged", "Fragment")]
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
                    bigBlock = new FixedBuffer<T>(largeBlockSize);
                    FixedBuffer<T> smallBlock = new FixedBuffer<T>(SmallBlockSize);
                    int j = JemUtil.Rng.Next(0, ArraySize); 
                    T r = GM<T>.Random();
                    smallBlock[j] = r;
                    smallBlocks[i] = smallBlock;
                    if (!smallBlocks[i][j].Equals(r))
                    {
                        throw new Exception($"Cannot validate small block at index {i}.");
                    }
                    if (!bigBlock.Free()) throw new Exception("Cannot free bigBlock.");
                    largeBlockSize = largeBlockSize + (1 * 1024 * 1024);
                }
                this.SetStatistic($"{nameof(FragmentNativeHeap)}_WorkingSet", JemUtil.PrintBytes(JemUtil.ProcessWorkingSet));
                this.SetStatistic($"{nameof(FragmentNativeHeap)}_JemResident", JemUtil.PrintBytes(Jem.ResidentBytes));
                this.SetStatistic($"{nameof(FragmentNativeHeap)}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
                this.SetStatistic($"{nameof(FragmentNativeHeap)}_JemAllocated", JemUtil.PrintBytes(Jem.AllocatedBytes));
                foreach (FixedBuffer<T> b in smallBlocks)
                {
                    if (!b.Free()) throw new Exception($"Cannot free small block at index {i}.");

                }
            }
            catch (OutOfMemoryException)
            {
                Info(Jem.MallocStats);
                Error($"Out-of-Memory at index {i} with large block size {largeBlockSize}.");
                foreach (FixedBuffer<T> b in smallBlocks)
                {
                    b.Free();
                }
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion
    }
}
