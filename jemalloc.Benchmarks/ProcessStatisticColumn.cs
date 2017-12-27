using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Columns;
namespace jemalloc.Benchmarks
{
    public class ProcessMemoryStatisticColumn : IColumn
    {
        #region Constructors
        public ProcessMemoryStatisticColumn(string columnName, Func<string> jemStatFunction, string legend)
        {
            ColumnName = columnName;
            JemStatFunction = jemStatFunction;
            Legend = legend;
        }
        #endregion

        #region Implemented properties
        //
        // Summary:
        //     An unique identificator of the column. If there are several columns with the
        //     same Id, only one of them will be shown in the summary.
        public string Id => ColumnName;
        //
        // Summary:
        //     Defines order of column in the same category.
        public int PriorityInCategory { get; } = 99;

        // Summary:
        //     Defines how to format column's value
        public UnitType UnitType { get; } = UnitType.Size;
        //
        // Summary:
        //     Column description.
        public string Legend { get; protected set; }
        public bool AlwaysShow => true;

        public bool IsNumeric => true;

        public ColumnCategory Category => ColumnCategory.Statistics;

        #endregion

        #region Implemented methods
        public bool IsDefault(Summary summary, Benchmark benchmark) => false;

        public string GetValue(Summary summary, Benchmark benchmark) => JemStatFunction.Invoke();

        public bool IsAvailable(Summary summary) => Jem.Initialized;
        //
        // Summary:
        //     Value in this column formatted using the specified style.
        public string GetValue(Summary summary, Benchmark benchmark, ISummaryStyle style) => (JemStatFunction.Invoke());

        public string ColumnName { get; }
        #endregion

        #region Overriden methods
        public override string ToString() => ColumnName;
        #endregion

        #region Fields
        Func<string> JemStatFunction;
        #endregion

        #region Available columns
        public static readonly IColumn ProcessPrivateMemory = new JemStatisticColumn("Process allocated", () => JemUtil.PrintBytes(JemUtil.ProcessPrivateMemory),
            "Memory for entire process per single operation (native and managed, inclusive, 1KB = 1024B)");
       
        #endregion
    }
}
