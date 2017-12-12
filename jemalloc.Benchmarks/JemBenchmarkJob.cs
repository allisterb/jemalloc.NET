using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.InProcess;

namespace jemalloc.Benchmarks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class JemBenchmarkJobAttribute : Attribute, IConfigSource
    {
        public JemBenchmarkJobAttribute(
            RunStrategy RunStrategy = RunStrategy.Throughput,
            int TargetCount = DefaultValue,
            int InvocationCount = DefaultValue,
            int WarmupCount = DefaultValue
            )
        {
            Job job = new Job()
                .WithGcAllowVeryLargeObjects(true)
                .WithId("JemBenchmark");
            job.Env.Platform = Platform.X64;
            job.Env.Runtime = Runtime.Core;
            job.Env.Jit = Jit.RyuJit;


            if (WarmupCountOverride.HasValue)
            {
                job.Run.WarmupCount = WarmupCountOverride.Value;
            }
            else if (WarmupCount != DefaultValue)
            {
                job.Run.WarmupCount = WarmupCount;
            }
            

            if (TargetCountOverride.HasValue)
            {
                job.Run.TargetCount = TargetCountOverride.Value;
            }
            else if (TargetCount != DefaultValue)
            {
                job.Run.TargetCount = TargetCount;
            }

            if (InvocationCountOverride.HasValue)
            {
                job.Run.InvocationCount = InvocationCountOverride.Value;
            }
            else if (InvocationCount != DefaultValue)
            {
                job.Run.InvocationCount = InvocationCount;
            }

            if (ColdStartOverride.HasValue)
            {
                job.Run.RunStrategy = ColdStartOverride.Value ? RunStrategy.ColdStart : RunStrategy.Throughput;
            }
            else 
            {
                job.Run.RunStrategy = RunStrategy;
            }
            

            job.Infrastructure.Toolchain = new InProcessToolchain(TimeSpan.FromMinutes(TimeoutInMinutes), BenchmarkActionCodegen.ReflectionEmit, true);
            Config = ManualConfig.CreateEmpty().With(job);
        }

        #region Properties
        public IConfig Config { get; }
        public int TimeoutInMinutes { get; set; } = 10;
        public static int? TargetCountOverride { get; set; } = null;
        public static int? WarmupCountOverride { get; set; } = null;
        public static int? InvocationCountOverride { get; set; } = null;
        public static bool? ColdStartOverride { get; set; } = null;
        #endregion

        #region Fields
        private const int DefaultValue = -1;
        #endregion
    }
}
