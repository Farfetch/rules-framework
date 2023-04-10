namespace Rules.Framework.BenchmarkTests.Exporters.Common
{
    internal class BenchmarkStatisticsComparisonItem
    {
        public BenchmarkStatisticsValue? AllocatedMemoryRate { get; set; }

        public BenchmarkStatisticsValue? BaselineAllocatedMemory { get; set; }

        public BenchmarkStatisticsValue? BaselineMeanTimeTaken { get; set; }

        public string? BaselineParameters { get; set; }

        public BenchmarkStatisticsValue? CompareAllocatedMemory { get; set; }

        public BenchmarkStatisticsValue? CompareMeanTimeTaken { get; set; }

        public string? CompareParameters { get; set; }

        public string? Key { get; set; }

        public BenchmarkStatisticsValue? MeanTimeTakenCompareRate { get; set; }
    }
}