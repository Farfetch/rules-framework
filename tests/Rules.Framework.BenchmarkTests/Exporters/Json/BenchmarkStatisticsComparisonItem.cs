namespace Rules.Framework.BenchmarkTests.Exporters.Json
{
    internal class BenchmarkStatisticsComparisonItem
    {
        public BenchmarkStatisticsValue? AllocatedMemoryRate { get; set; }

        public BenchmarkStatisticsValue? BaselineAllocatedMemory { get; set; }

        public BenchmarkStatisticsValue? BaselineMeanTimeTaken { get; set; }

        public string BaselineParameters { get; set; } = string.Empty;

        public BenchmarkStatisticsValue? CompareAllocatedMemory { get; set; }

        public BenchmarkStatisticsValue? CompareMeanTimeTaken { get; set; }

        public string CompareParameters { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public BenchmarkStatisticsValue? MeanTimeTakenCompareRate { get; set; }
    }
}