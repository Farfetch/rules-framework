namespace Rules.Framework.BenchmarkTests
{
    using System;
    using BenchmarkDotNet.Columns;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    internal class CustomBaselineClassifierColumn : IColumn
    {
        private readonly Func<BenchmarkCase, bool> classifyLogicFunc;

        public CustomBaselineClassifierColumn(Func<BenchmarkCase, bool> classifyLogicFunc)
        {
            this.classifyLogicFunc = classifyLogicFunc;
        }

        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Baseline;
        public string ColumnName => "Baseline";
        public string Id => nameof(CustomBaselineClassifierColumn);
        public bool IsNumeric => false;
        public string Legend => "Sets wether a test case is a baseline.";
        public int PriorityInCategory => 0;
        public UnitType UnitType => UnitType.Dimensionless;

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
            => this.classifyLogicFunc.Invoke(benchmarkCase) ? "Yes" : "No";

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
            => this.GetValue(summary, benchmarkCase);

        public bool IsAvailable(Summary summary) => true;

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    }
}