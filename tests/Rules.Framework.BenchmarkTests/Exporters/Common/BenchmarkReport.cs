namespace Rules.Framework.BenchmarkTests.Exporters.Common
{
    using System;

    internal class BenchmarkReport
    {
        public DateTime Date { get; set; }

        public Environment? Environment { get; set; }

        public IEnumerable<BenchmarkStatisticsItem>? Statistics { get; set; }

        public IEnumerable<BenchmarkStatisticsComparisonItem>? StatisticsComparison { get; set; }

        public string Title { get; set; } = string.Empty;
    }
}