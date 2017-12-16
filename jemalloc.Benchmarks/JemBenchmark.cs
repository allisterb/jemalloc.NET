using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using System.Threading;
using BenchmarkDotNet;
using BenchmarkDotNet.Order;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Code;
using BenchmarkDotNet.Loggers;

namespace jemalloc.Benchmarks
{
    [JemBenchmarkJob]
    [MemoryDiagnoser]
    public abstract class JemBenchmark<TData, TParam> where TData : struct, IEquatable<TData>, IComparable<TData>, IConvertible where TParam : struct
    {
        #region Constructors
        static JemBenchmark()
        {

        }
        public JemBenchmark() { }
        #endregion

        #region Properties
        [ParamsSource(nameof(GetParameters))]
        public TParam Parameter;

        public static IEnumerable<TParam> BenchmarkParameters { get; set; }

        public static ILogger Log { get; } = new ConsoleLogger();

        public static Process CurrentProcess { get; } = Process.GetCurrentProcess();

        public static long InitialPrivateMemorySize { get; protected set; }

        public static long PrivateMemorySize
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PrivateMemorySize64;
            }
        }

        public static long PeakWorkingSet
        {
            get
            {
                CurrentProcess.Refresh();
                return CurrentProcess.PeakWorkingSet64;
            }
        }
        #endregion

        #region Methods
        public IEnumerable<IParam> GetParameters() => BenchmarkParameters.Select(p => new JemBenchmarkParam<TParam>(p));

        public static int GetBenchmarkMethodCount<TBench>() where TBench : JemBenchmark<TData, TParam>
        {
            return typeof(TBench).GenericTypeArguments.First().GetMethods(BindingFlags.Public).Count();
        }

        public virtual void GlobalSetup()
        {
            CurrentProcess.Refresh();
            InitialPrivateMemorySize = CurrentProcess.PeakWorkingSet64;
        }

        public static void SetColdStartOverride(bool value)
        {
            JemBenchmarkJobAttribute.ColdStartOverride = value;
        }

        public static void SetTargetCountOverride(int value)
        {
            JemBenchmarkJobAttribute.TargetCountOverride = value;
        }

        public static void SetInvocationCountOverride(int value)
        {
            JemBenchmarkJobAttribute.InvocationCountOverride = value;
        }

        public static void SetWarmupCountOverride(int value)
        {
            JemBenchmarkJobAttribute.WarmupCountOverride = value;
        }

        public TValue GetValue<TValue>(string name, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (JemUtil.BenchmarkValues.TryGetValue($"{Thread.CurrentThread.ManagedThreadId}_{name}", out object v))
            {
                return (TValue) v;
            }
            else throw new Exception($"Could not get value {name}.");
        }

        public TValue SetValue<TValue>(string name, TValue value, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (!JemUtil.BenchmarkValues.TryAdd($"{Thread.CurrentThread.ManagedThreadId}_{name}", value))
            {
                throw new Exception("Could not add value.");
            }
            return value;
        }

        public void RemoveValue(string name, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            JemUtil.BenchmarkValues.Remove($"{Thread.CurrentThread.ManagedThreadId}_{name}", out object o);
        }

        #region Log
        public static void Info(string format, params object[] values) => Log.WriteLineInfo(string.Format(format, values));

        public static void InfoThis([CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) => Info("Executing {0}().", memberName);
        #endregion

        public static TData GetArrayFillValue() => GM<TData>.Random();            
        
        public static TData GetArrayMulValue() => GM<TData>.Const(4);

        public static TData GetArraySqrFillValue() => GM<TData>.Random();
        #endregion
    }
}
