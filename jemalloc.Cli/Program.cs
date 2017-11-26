using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Serilog;
using CommandLine;
using CommandLine.Text;

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

        static Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        static LoggerConfiguration LConfig;
        static ILogger L;

        static void Main(string[] args)
        {
            
            LConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss}<{ThreadId:d2}> [{Level:u3}] {Message}{NewLine}{Exception}");
            L = Log.Logger = LConfig.CreateLogger();
            ParserResult<object> result = new Parser().ParseArguments<Options, MallocOptions>(args);
            result.WithNotParsed((IEnumerable<Error> errors) =>
            {
                HelpText help = GetAutoBuiltHelpText(result);
                help.Heading = new HeadingInfo("jemalloc.NET", Version.ToString(3));
                help.Copyright = string.Empty;
                help.AddPreOptionsLine(string.Empty);
                
                if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
                {
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
                    help.AddVerbs(typeof(MallocOptions));
                    L.Information(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
                {
                    help.AddVerbs(typeof(MallocOptions));
                    L.Error("No operation selected. Specify one of: malloc, calloc, gen.");
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.MissingRequiredOptionError))
                {
                    MissingRequiredOptionError error = (MissingRequiredOptionError)errors.First(e => e.Tag == ErrorType.MissingRequiredOptionError);
                    L.Error("A required option is missing: {0}.", error.NameInfo.NameText);
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.UnknownOptionError))
                {
                    UnknownOptionError error = (UnknownOptionError)errors.First(e => e.Tag == ErrorType.UnknownOptionError);
                    help.AddVerbs(typeof(MallocOptions));
                    L.Error("Unknown option: {error}.", error.Token);
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else
                {
                    L.Error("An error occurred parsing the program options: {errors}.", errors);
                    help.AddVerbs(typeof(MallocOptions));
                    L.Information(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
            });


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

    }
}
