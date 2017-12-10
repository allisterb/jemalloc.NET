using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;

namespace jemalloc.Benchmarks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class JemBenchmarkJobAttribute : Attribute, IConfigSource
    {
        public JemBenchmarkJobAttribute()
        {
            Job job = new Job("JemBenchmark", InfrastructureMode.InProcess, EnvMode.RyuJitX64);
            job.Env.Platform = Platform.X64;
            job.Env.Runtime = Runtime.Core;
            Config = ManualConfig.CreateEmpty().With(job);
        }

        public IConfig Config { get; }
    }
}
