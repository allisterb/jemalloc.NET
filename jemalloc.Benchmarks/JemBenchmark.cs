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
    #region Enums
    public enum Category
    {
        MALLOC,
        NARRAY,
        HUGEARRAY,
        BUFFER,
        VECTOR
    }

    public enum Operation
    {
        CREATE,
        FILL,
        MATH,
        FRAGMENT,
        MANDELBROT,
        TEST
    }
    #endregion

    [JemBenchmarkJob]
    [MemoryDiagnoser]
    public abstract class JemBenchmark<TData, TParam> where TData : struct, IEquatable<TData>, IComparable<TData>, IConvertible where TParam : struct
    {
        #region Constructors
        static JemBenchmark()
        {

        }
        public JemBenchmark()
        {
            
        }
        #endregion

        #region Properties
        [ParamsSource(nameof(GetParameters))]
        public TParam Parameter;

        public static List<TParam> BenchmarkParameters { get; set; }

        public static Category Category { get; set; }

        public static Operation Operation { get; set; }

        public static bool Debug { get; set; }

        public static bool Validate { get; set; }

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
        public IEnumerable<IParam> GetParameters() 
        {
            IEnumerable<IParam> param;
            param = BenchmarkParameters.Select(p => new JemBenchmarkParam<TParam>(p));
            return param;
        }
        public static int GetBenchmarkMethodCount<TBench>() where TBench : JemBenchmark<TData, TParam>
        {
            return typeof(TBench).GenericTypeArguments.First().GetMethods(BindingFlags.Public).Count();
        }

        [GlobalSetup]
        public virtual void GlobalSetup()
        {
            DebugInfoThis();
            Info("Data type is {0}.", typeof(TData).Name);
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
            if (JemUtil.BenchmarkValues.TryGetValue($"{name}_{Parameter.GetHashCode()}", out object v))
            {
                return (TValue) v;
            }
            else throw new Exception($"Could not get value {name}.");
        }

        public void SetValue<TValue>(string name, TValue value, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            JemUtil.BenchmarkValues.GetOrAdd($"{name}_{Parameter.GetHashCode()}", value);
         
        }

        public void RemoveValue(string name, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            JemUtil.BenchmarkValues.Remove($"{name}_{Parameter.GetHashCode()}", out object o);
        }

        public void SetStatistic(string name, string value, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            JemUtil.BenchmarkStatistics.AddOrUpdate($"{memberName}_{name}", value, ((k, v) => value));
        }

        public void SetMemoryStatistics([CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            this.SetStatistic($"{memberName}_WorkingSet", JemUtil.PrintBytes(JemUtil.ProcessWorkingSet));
            this.SetStatistic($"{memberName}_JemResident", JemUtil.PrintBytes(Jem.ResidentBytes));
            this.SetStatistic($"{memberName}_PrivateMemory", JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory));
            this.SetStatistic($"{memberName}_JemAllocated", JemUtil.PrintBytes(Jem.AllocatedBytes));
        }


        #region Log
        public static void Info(string format, params object[] values) => Log.WriteLineInfo(string.Format(format, values));

        public static void DebugInfo(string format, params object[] values)
        {
            if (Debug)
            {
                Info(format, values);
            }
        }

        public static void InfoThis([CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) => Info("Executing {0}() on thread {1}.", memberName, Thread.CurrentThread.ManagedThreadId);

        public static void DebugInfoThis([CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (Debug)
            {
                InfoThis(memberName, fileName, lineNumber);
            }
        }

        public static void Error(string text, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) => 
            Log.WriteLineError(string.Format("Error : {0} At {1} in {2} on line {3}.", text, memberName, fileName, lineNumber));

        public static void Error(string format, params object[] values) => Log.WriteLineError(string.Format(format, values));
        #endregion

        public static string PrintBytes(double bytes, string suffix = "")
        {
            if (bytes >= 0 && bytes <= 1024)
            {
                return string.Format("{0:N0} B{1}", bytes, suffix);
            }
            else if (bytes >= 1024 && bytes < (1024 * 1024))
            {
                return string.Format("{0:N1} KB{1}", bytes / 1024, suffix);
            }
            else if (bytes >= (1024 * 1024) && bytes < (1024 * 1024 * 1024))
            {
                return string.Format("{0:N1} MB{1}", bytes / (1024 * 1024), suffix);
            }
            else if (bytes >= (1024 * 1024 * 1024))
            {
                return string.Format("{0:N1} GB{1}", bytes / (1024 * 1024 * 1024), suffix);
            }
            else throw new ArgumentOutOfRangeException();

        }

        public static Tuple<double, string> PrintBytesToTuple(double bytes, string suffix = "")
        {
            string[] s = PrintBytes(bytes, suffix).Split(' ');
            return new Tuple<double, string>(Double.Parse(s[0]), s[1]);
        }

        private static object benchmarkLock = new object();
        #endregion
    }
}
