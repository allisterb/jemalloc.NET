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
        public JemBenchmarkJobAttribute()
        {
            Job job = new Job("JemBenchmark");
            job.Env.Platform = Platform.X64;
            job.Env.Runtime = Runtime.Core;
            job.Env.Jit = Jit.RyuJit;
            job.Run.WarmupCount = 1;
            job.Run.TargetCount = 1;
            job.Run.RunStrategy = RunStrategy.ColdStart;
            job.Infrastructure.Toolchain = new InProcessToolchain(TimeSpan.FromMinutes(5), BenchmarkActionCodegen.ReflectionEmit, true);
            Config = ManualConfig.CreateEmpty().With(job);
        }

        public IConfig Config { get; }
    }
}
