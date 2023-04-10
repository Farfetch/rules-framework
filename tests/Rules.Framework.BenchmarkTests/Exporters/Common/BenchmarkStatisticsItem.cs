namespace Rules.Framework.BenchmarkTests.Exporters.Common
{
    internal class BenchmarkStatisticsItem
    {
        public BenchmarkStatisticsItem(
            string key,
            string parameters,
            string baseline,
            BenchmarkStatisticsValue meanTimeTaken,
            BenchmarkStatisticsValue standardError,
            BenchmarkStatisticsValue branchInstructionsPerOp,
            BenchmarkStatisticsValue branchMispredictionsPerOp,
            BenchmarkStatisticsValue gen0Collects,
            BenchmarkStatisticsValue allocatedMemory)
        {
            this.Key = key;
            this.Parameters = parameters;
            this.Baseline = baseline;
            this.MeanTimeTaken = meanTimeTaken;
            this.StandardError = standardError;
            this.BranchInstructionsPerOp = branchInstructionsPerOp;
            this.BranchMispredictionsPerOp = branchMispredictionsPerOp;
            this.Gen0Collects = gen0Collects;
            this.AllocatedMemory = allocatedMemory;
        }

        public BenchmarkStatisticsValue AllocatedMemory { get; set; }

        public string Baseline { get; set; }

        public BenchmarkStatisticsValue BranchInstructionsPerOp { get; set; }

        public BenchmarkStatisticsValue BranchMispredictionsPerOp { get; set; }

        public BenchmarkStatisticsValue Gen0Collects { get; set; }

        public string Key { get; set; }

        public BenchmarkStatisticsValue MeanTimeTaken { get; set; }

        public string Parameters { get; set; }

        public BenchmarkStatisticsValue StandardError { get; set; }

        public static BenchmarkStatisticsItem New(
            string key,
            string parameters,
            string baseline,
            BenchmarkStatisticsValue meanTimeTaken,
            BenchmarkStatisticsValue standardError,
            BenchmarkStatisticsValue branchInstructionsPerOp,
            BenchmarkStatisticsValue branchMispredictionsPerOp,
            BenchmarkStatisticsValue gen0Collects,
            BenchmarkStatisticsValue allocatedMemory)
            => new BenchmarkStatisticsItem(key,
                                           parameters,
                                           baseline,
                                           meanTimeTaken,
                                           standardError,
                                           branchInstructionsPerOp,
                                           branchMispredictionsPerOp,
                                           gen0Collects,
                                           allocatedMemory);
    }
}