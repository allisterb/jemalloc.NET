using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using Serilog;
using CommandLine;
using CommandLine.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Reports;

using jemalloc.Benchmarks;

namespace jemalloc.Cli
{
    class Program
    {
        public enum ExitResult
        {
            SUCCESS = 0,
            UNHANDLED_EXCEPTION = 1,
            INVALID_OPTIONS = 2
        }

        public enum Category
        {
            MALLOC,
            NARRAY,
            HUGEARRAY,
            BUFFER
        }

        public enum Operation
        {
            CREATE,
            FILL,
            MATH,
            FRAGMENT
        }

        static Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        static LoggerConfiguration LConfig;
        static ILogger L;
        static Dictionary<string, object> BenchmarkOptions = new Dictionary<string, object>();
        static Summary BenchmarkSummary;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            LConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss}<{ThreadId:d2}> [{Level:u3}] {Message}{NewLine}{Exception}");
            L = Log.Logger = LConfig.CreateLogger();
            Type[] BenchmarkOptionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Options))).ToArray();
            MethodInfo parseArgumentsMethod = typeof(ParserExtensions).GetMethods().Where(m => m.IsGenericMethod && m.Name == "ParseArguments" && m.GetGenericArguments().Count() == BenchmarkOptionTypes.Count()).First();
            Parser p = new Parser();
            ParserResult<object> result = (ParserResult<object>) parseArgumentsMethod.MakeGenericMethod(BenchmarkOptionTypes).Invoke(p , new object[] { p, args });
            result.WithNotParsed((IEnumerable<Error> errors) =>
            {
                HelpText help = GetAutoBuiltHelpText(result);
                help.MaximumDisplayWidth = Console.WindowWidth;
                help.Copyright = string.Empty;
                help.Heading = new HeadingInfo("jemalloc.NET", Version.ToString(3));
                help.AddPreOptionsLine(string.Empty);
                if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
                {
                    Log.Information(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError))
                {
                    HelpVerbRequestedError error = (HelpVerbRequestedError)errors.First(e => e.Tag == ErrorType.HelpVerbRequestedError);
                    if (error.Type != null)
                    {
                        help.AddVerbs(error.Type);
                    }
                    Log.Information(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
                {
                    help.AddVerbs(BenchmarkOptionTypes);
                    L.Information(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
                {
                    help.AddVerbs(BenchmarkOptionTypes);
                    help.AddPreOptionsLine("No category selected. Select a category from the options below:");
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.MissingRequiredOptionError))
                {
                    MissingRequiredOptionError error = (MissingRequiredOptionError)errors.First(e => e is MissingRequiredOptionError);
                    help.AddOptions(result);
                    help.AddPreOptionsLine($"A required option or value is missing. The options and values for this benchmark category are: ");
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.MissingValueOptionError))
                {
                    MissingValueOptionError error = (MissingValueOptionError)errors.First(e => e.Tag == ErrorType.MissingValueOptionError);
                    help.AddOptions(result);
                    help.AddPreOptionsLine($"A required option or value is missing. The options and values for this benchmark category are: ");
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.UnknownOptionError))
                {
                    UnknownOptionError error = (UnknownOptionError)errors.First(e => e.Tag == ErrorType.UnknownOptionError);
                    help.AddOptions(result);
                    help.AddPreOptionsLine($"Unknown option: {error.Token}.");
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else
                {
                    help.AddPreOptionsLine($"An error occurred parsing the program options: {string.Join(' ', errors.Select(e => e.Tag.ToString()).ToArray())}");
                    help.AddVerbs(BenchmarkOptionTypes);
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
            })
            .WithParsed((Options o) =>
            {
              
                foreach (PropertyInfo prop in o.GetType().GetProperties())
                {
                    BenchmarkOptions.Add(prop.Name, prop.GetValue(o));
                }
                if (o.ColdStart)
                {
                    JemBenchmarkJobAttribute.ColdStartOverride = true;
                }
                if (o.TargetCount > 0)
                {
                    JemBenchmarkJobAttribute.TargetCountOverride = o.TargetCount;
                }
            })
            .WithParsed<MallocBenchmarkOptions>(o =>
            {
                BenchmarkOptions.Add("Category", Category.MALLOC);
                if (o.Create)
                {
                    BenchmarkOptions.Add("Operation", Operation.CREATE);
                }
                else if (o.Fill)
                {
                    BenchmarkOptions.Add("Operation", Operation.FILL);
                }
                else if (o.Fragment)
                {
                    BenchmarkOptions.Add("Operation", Operation.FRAGMENT);
                }
                if (!BenchmarkOptions.ContainsKey("Operation"))
                {
                    Log.Error("You must select an operation to benchmark with --fill.");
                    Exit(ExitResult.SUCCESS);
                }
                else
                {
                    Benchmark(o);
                }
            })
            .WithParsed<FixedBufferBenchmarkOptions>(o =>
            {
                BenchmarkOptions.Add("Category", Category.BUFFER);
                if (o.Create)
                {
                    BenchmarkOptions.Add("Operation", Operation.CREATE);
                }
                else if (o.Fill)
                {
                    BenchmarkOptions.Add("Operation", Operation.FILL);
                }
                else if (o.Math)
                {
                    BenchmarkOptions.Add("Operation", Operation.MATH);
                }

                if (!BenchmarkOptions.ContainsKey("Operation"))
                {
                    Log.Error("You must select an operation to benchmark with --create or --fill.");
                    Exit(ExitResult.SUCCESS);
                }
                else
                {
                    Benchmark(o);
                }

            })
            .WithParsed<SafeArrayBenchmarkOptions>(o =>
            {
                BenchmarkOptions.Add("Category", Category.NARRAY);
                if (o.Create)
                {
                    BenchmarkOptions.Add("Operation", Operation.CREATE);
                }
                else if (o.Fill)
                {
                    BenchmarkOptions.Add("Operation", Operation.FILL);
                }
                else if (o.Math)
                {
                    BenchmarkOptions.Add("Operation", Operation.MATH);
                }

                if (!BenchmarkOptions.ContainsKey("Operation"))
                {
                    Log.Error("You must select an operation to benchmark with --create or --fill.");
                    Exit(ExitResult.SUCCESS);
                }
                else
                {
                    Benchmark(o);
                }
                
            })
            .WithParsed<HugeNativeArrayBenchmarkOptions>(o =>
            {
                BenchmarkOptions.Add("Category", Category.HUGEARRAY);
                if (o.Create)
                {
                    BenchmarkOptions.Add("Operation", Operation.CREATE);
                }
                else if (o.Fill)
                {
                    BenchmarkOptions.Add("Operation", Operation.FILL);
                }
                else if (o.Math)
                {
                    BenchmarkOptions.Add("Operation", Operation.MATH);
                }

                if (!BenchmarkOptions.ContainsKey("Operation"))
                {
                    Log.Error("You must select an operation to benchmark with --create or --fill or --math.");
                    Exit(ExitResult.SUCCESS);
                }
                else
                {
                    Benchmark(o);
                }
            }); 
        }

        static void Benchmark(Options o)
        {
            Contract.Requires(BenchmarkOptions.ContainsKey("Operation"));
            Contract.Requires(BenchmarkOptions.ContainsKey("Category"));
            Contract.Requires(BenchmarkOptions.ContainsKey("Sizes"));
            if (o.Int8 && o.Unsigned)
            {
                Benchmark<Byte>();
            }
            else if (o.Int8)
            {
                Benchmark<SByte>();
            }
            if (o.Int16 && o.Unsigned)
            {
                Benchmark<UInt16>();
            }
            else if (o.Int16)
            {
                Benchmark<Int16>();
            }
            else if (o.Int32 && o.Unsigned)
            {
                Benchmark<UInt32>();
            }
            else if (o.Int32)
            {
                Benchmark<Int32>();
            }
            else if (o.Int64 && o.Unsigned)
            {
                Benchmark<UInt64>();
            }
            else if (o.Int64)
            {
                Benchmark<Int64>();
            }
            else if (o.Double)
            {
                Benchmark<Double>();
            }
            else
            {
                L.Error("You must select a data type to benchmark with: -b, -i, -h, -l, -d, -s, and -u.");
                Exit(ExitResult.INVALID_OPTIONS);
            }
        }
        static void Benchmark<T>() where T : struct, IEquatable<T>, IComparable<T>, IConvertible
        {
            Contract.Requires(BenchmarkOptions.ContainsKey("Category"));
            Contract.Requires(BenchmarkOptions.ContainsKey("Operation"));
            Contract.Requires(BenchmarkOptions.ContainsKey("Sizes"));
            IConfig config = ManualConfig
                         .Create(DefaultConfig.Instance);
            JemBenchmarkAttribute.CurrentConfig = config;
            try
            {
                switch ((Category)BenchmarkOptions["Category"])
                {
                    case Category.MALLOC:
                        MallocVsArrayBenchmark<T>.BenchmarkParameters = (IEnumerable<int>)BenchmarkOptions["Sizes"];
                        switch ((Operation)BenchmarkOptions["Operation"])
                        {
                            case Operation.CREATE:
                                config = config.With(new NameFilter(name => name.Contains("Create")));
                                L.Information("Starting {num} create benchmarks for data type {t} with array sizes: {s}", JemBenchmark<T, int>.GetBenchmarkMethodCount<MallocVsArrayBenchmark<T>>(),
                                    typeof(T).Name, MallocVsArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                break;

                            case Operation.FILL:
                                config = config.With(new NameFilter(name => name.Contains("Fill")));
                                L.Information("Starting {num} fill benchmarks for data type {t} with array sizes: {s}", JemBenchmark<T, int>.GetBenchmarkMethodCount<MallocVsArrayBenchmark<T>>(),
                                    typeof(T).Name, MallocVsArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");                        
                                break;

                            case Operation.FRAGMENT:
                                config = config.With(new NameFilter(name => name.Contains("Fragment")));
                                L.Information("Starting {num} fragment benchmarks for data type {t} with array sizes: {s}", JemBenchmark<T, int>.GetBenchmarkMethodCount<MallocVsArrayBenchmark<T>>(),
                                    typeof(T).Name, MallocVsArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown operation: {(Operation)BenchmarkOptions["Operation"]} for category {(Category)BenchmarkOptions["Category"]}.");
                        }
                        BenchmarkSummary = BenchmarkRunner.Run<MallocVsArrayBenchmark<T>>(config);
                        break;

                    case Category.BUFFER:
                        FixedBufferVsManagedArrayBenchmark<T>.BenchmarkParameters = (IEnumerable<int>)BenchmarkOptions["Sizes"];
                        switch ((Operation)BenchmarkOptions["Operation"])
                        {
                            case Operation.CREATE:
                                L.Information("Starting {num} create array benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, int>.GetBenchmarkMethodCount<FixedBufferVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name,
                                    FixedBufferVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                config = config
                                    .With(new NameFilter(name => name.StartsWith("Create")));
                                break;

                            case Operation.FILL:
                                L.Information("Starting {num} array fill benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, int>.GetBenchmarkMethodCount<FixedBufferVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, FixedBufferVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                config = config
                                    .With(new NameFilter(name => name.StartsWith("Fill")));
                                break;

                            case Operation.MATH:
                                L.Information("Starting {num} array math benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, int>.GetBenchmarkMethodCount<FixedBufferVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, FixedBufferVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                config = config
                                    .With(new NameFilter(name => name.StartsWith("Arith")));
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown operation: {(Operation)BenchmarkOptions["Operation"]} for category {(Category)BenchmarkOptions["Category"]}.");
                        }
                        BenchmarkSummary = BenchmarkRunner.Run<FixedBufferVsManagedArrayBenchmark<T>>(config);
                        break;

                    case Category.NARRAY:
                        SafeVsManagedArrayBenchmark<T>.BenchmarkParameters = (IEnumerable<int>)BenchmarkOptions["Sizes"];
                        switch ((Operation)BenchmarkOptions["Operation"])
                        {
                            case Operation.CREATE:
                                L.Information("Starting {num} create array benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, int>.GetBenchmarkMethodCount<SafeVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name,
                                    SafeVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                config = config
                                    .With(new NameFilter(name => name.StartsWith("Create")));
                                break;

                            case Operation.FILL:
                                L.Information("Starting {num} array fill benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, int>.GetBenchmarkMethodCount<SafeVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, SafeVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                config = config
                                    .With(new NameFilter(name => name.StartsWith("Fill")));
                                break;

                            case Operation.MATH:
                                L.Information("Starting {num} array math benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, int>.GetBenchmarkMethodCount<SafeVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, SafeVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("Please allow some time for the pilot and warmup phases of the benchmark.");
                                config = config
                                    .With(new NameFilter(name => name.StartsWith("Arith")));
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown operation: {(Operation)BenchmarkOptions["Operation"]} for category {(Category)BenchmarkOptions["Category"]}.");
                        }
                        BenchmarkSummary = BenchmarkRunner.Run<SafeVsManagedArrayBenchmark<T>>(config);
                        break;

                    case Category.HUGEARRAY:
                        HugeNativeVsManagedArrayBenchmark<T>.BenchmarkParameters = (IEnumerable<ulong>)BenchmarkOptions["Sizes"];
                        switch ((Operation)BenchmarkOptions["Operation"])
                        {
                            case Operation.CREATE:
                                config = config.With(new NameFilter(name => name.Contains("Create")));
                                L.Information("Starting {num} huge array create benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, ulong>.GetBenchmarkMethodCount<HugeNativeVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, HugeNativeVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("This benchmark does not have a warmup phase but can still take a while (10-15 minutes.)");
                                break;

                            case Operation.FILL:
                                config = config.With(new NameFilter(name => name.Contains("Fill")));
                                L.Information("Starting {num} huge array fill benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, ulong>.GetBenchmarkMethodCount<HugeNativeVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, HugeNativeVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("This benchmark does not have a warmup phase but can still take a while (10-15 minutes.)");
                                break;

                            case Operation.MATH:
                                config = config.With(new NameFilter(name => name.Contains("Arithmetic")));
                                L.Information("Starting {num} huge array math benchmarks for data type {t} with array sizes: {s}",
                                    JemBenchmark<T, ulong>.GetBenchmarkMethodCount<HugeNativeVsManagedArrayBenchmark<T>>(),
                                    typeof(T).Name, HugeNativeVsManagedArrayBenchmark<T>.BenchmarkParameters);
                                L.Information("This benchmark does not have a warmup phase but can still take a while (10-15 minutes.)");
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown operation: {(Operation)BenchmarkOptions["Operation"]} for category {(Category)BenchmarkOptions["Category"]}.");
                        }
                        BenchmarkSummary = BenchmarkRunner.Run<HugeNativeVsManagedArrayBenchmark<T>>(config);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown category: {(Category)BenchmarkOptions["Category"]}.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception thrown during benchmark.");
                Exit(ExitResult.UNHANDLED_EXCEPTION);
            }
        }

        static void Exit(ExitResult result)
        {
            Log.CloseAndFlush();
            
            Environment.Exit((int)result);
        }

        static int ExitWithCode(ExitResult result)
        {
            Log.CloseAndFlush();
            return (int)result;
        }

        static HelpText GetAutoBuiltHelpText(ParserResult<object> result)
        {
            return HelpText.AutoBuild(result, h =>
            {
                h.AddOptions(result);
                return h;
            },
            e =>
            {
                return e;
            });
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
            Log.Error(exception, "An unhandled exception occurred. The program will now shutdown.");
            Log.Error(exception.StackTrace);
            Exit(ExitResult.UNHANDLED_EXCEPTION);
        }

    }
}
