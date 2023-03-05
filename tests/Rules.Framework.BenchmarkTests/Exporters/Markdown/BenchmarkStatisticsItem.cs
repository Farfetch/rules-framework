namespace Rules.Framework.BenchmarkTests.Exporters.Markdown
{
    internal class BenchmarkStatisticsItem
    {
        public BenchmarkStatisticsValue? AllocatedMemory { get; set; }

        public string Baseline { get; set; } = string.Empty;

        public BenchmarkStatisticsValue? BranchInstructionsPerOp { get; set; }

        public BenchmarkStatisticsValue? BranchMispredictionsPerOp { get; set; }

        public BenchmarkStatisticsValue? Gen0Collects { get; set; }

        public string Key { get; set; } = string.Empty;

        public BenchmarkStatisticsValue? MeanTimeTaken { get; set; }

        public string Parameters { get; set; } = string.Empty;

        public BenchmarkStatisticsValue? StandardError { get; set; }
    }
}