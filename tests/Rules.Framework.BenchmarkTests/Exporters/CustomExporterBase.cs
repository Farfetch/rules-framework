namespace Rules.Framework.BenchmarkTests.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Reports;
    using Rules.Framework.BenchmarkTests.Exporters.Common;

    internal abstract class CustomExporterBase : ExporterBase
    {
        protected static decimal CalculateRate(decimal baselineValue, decimal compareValue)
                    => ((baselineValue - compareValue) / baselineValue) * 100;

        protected virtual BenchmarkStatisticsComparisonItem CreateBenchmarkStatisticsComparisonItem(BenchmarkStatisticsItem baselineStatisticsItem, BenchmarkStatisticsItem nonBaselineStatisticsItem) => new BenchmarkStatisticsComparisonItem
        {
            AllocatedMemoryRate = BenchmarkStatisticsValue.New(
                "0.##", "%", CalculateRate(baselineStatisticsItem.AllocatedMemory!.Value, nonBaselineStatisticsItem.AllocatedMemory!.Value)),
            BaselineAllocatedMemory = baselineStatisticsItem.AllocatedMemory,
            BaselineMeanTimeTaken = baselineStatisticsItem.MeanTimeTaken,
            BaselineParameters = baselineStatisticsItem.Parameters,
            CompareAllocatedMemory = nonBaselineStatisticsItem.AllocatedMemory,
            CompareMeanTimeTaken = nonBaselineStatisticsItem.MeanTimeTaken,
            CompareParameters = nonBaselineStatisticsItem.Parameters,
            Key = baselineStatisticsItem.Key,
            MeanTimeTakenCompareRate = BenchmarkStatisticsValue.New(
                "0.##", "%", CalculateRate(baselineStatisticsItem.MeanTimeTaken!.Value, nonBaselineStatisticsItem.MeanTimeTaken!.Value))
        };

        protected virtual BenchmarkStatisticsItem CreateBenchmarkStatisticsItem(SummaryTable.SummaryTableColumn baselineClassifierColumn, ref int current, BenchmarkDotNet.Reports.BenchmarkReport benchmarkReport)
        {
            var allocatedMemoryMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "Allocated Memory");
            var branchInstructionsMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "BranchInstructions");
            var branchMispredictionsMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "BranchMispredictions");
            var gen0CollectsMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "Gen0Collects");

            var statisticsItem = BenchmarkStatisticsItem.New(
                $"{benchmarkReport.BenchmarkCase.Descriptor.Type.Name}.{benchmarkReport.BenchmarkCase.Descriptor.WorkloadMethod.Name}",
                benchmarkReport.BenchmarkCase.Parameters.DisplayInfo,
                baselineClassifierColumn.Content[current++],
                BenchmarkStatisticsValue.New("N0", "ns", Convert.ToDecimal(benchmarkReport.ResultStatistics?.Mean ?? 0)),
                BenchmarkStatisticsValue.New("N0", "ns", Convert.ToDecimal(benchmarkReport.ResultStatistics?.StandardError ?? 0)),
                CreateBenchmarkStatisticsValue(branchInstructionsMetric.Value),
                CreateBenchmarkStatisticsValue(branchMispredictionsMetric.Value),
                CreateBenchmarkStatisticsValue(gen0CollectsMetric.Value),
                CreateBenchmarkStatisticsValue(allocatedMemoryMetric.Value)
                );

            return statisticsItem;
        }

        protected virtual BenchmarkStatisticsValue CreateBenchmarkStatisticsValue(Metric? allocatedMemoryMetric)
        {
            if (allocatedMemoryMetric is null)
            {
                return BenchmarkStatisticsValue.New(string.Empty, string.Empty, decimal.Zero);
            }

            return BenchmarkStatisticsValue.New(
                allocatedMemoryMetric.Descriptor.NumberFormat,
                allocatedMemoryMetric.Descriptor.Unit,
                Convert.ToDecimal(allocatedMemoryMetric.Value));
        }

        protected virtual Common.Environment CreateEnvironment(Summary summary) => new Common.Environment
        {
            Architecture = summary.HostEnvironmentInfo.Architecture,
            BenchmarkDotNetCaption = "BenchmarkDotNet",
            BenchmarkDotNetVersion = summary.HostEnvironmentInfo.BenchmarkDotNetVersion,
            BuildConfiguration = summary.HostEnvironmentInfo.Configuration,
            DotNetCliVersion = summary.HostEnvironmentInfo.DotNetSdkVersion.Value,
            DotNetRuntimeVersion = summary.HostEnvironmentInfo.RuntimeVersion,
            LogicalCoreCount = summary.HostEnvironmentInfo.CpuInfo.Value.LogicalCoreCount.GetValueOrDefault(0),
            PhysicalCoreCount = summary.HostEnvironmentInfo.CpuInfo.Value.PhysicalCoreCount.GetValueOrDefault(0),
            ProcessorName = summary.HostEnvironmentInfo.CpuInfo.Value.ProcessorName,
        };

        protected virtual Common.BenchmarkReport CreateReport(Summary summary)
        {
            var report = new Common.BenchmarkReport
            {
                Date = DateTime.UtcNow,
                Environment = CreateEnvironment(summary),
                Title = summary.Title,
            };
            var statistics = new List<BenchmarkStatisticsItem>(summary.Reports.Length);

            var baselineClassifierColumn = summary.Table.Columns.First(c => c.OriginalColumn.ColumnName == "Baseline");
            var current = 0;
            foreach (var benchmarkReport in summary.Reports)
            {
                var statisticsItem = CreateBenchmarkStatisticsItem(baselineClassifierColumn, ref current, benchmarkReport);

                statistics.Add(statisticsItem);
            }
            report.Statistics = statistics;

            var baselineStatisticsItems = statistics.Where(i => string.Equals(i.Baseline, "Yes"));
            var nonBaselineStatisticsItems = statistics.Where(i => string.Equals(i.Baseline, "No"));
            var statisticsComparison = new List<BenchmarkStatisticsComparisonItem>();

            foreach (var baselineStatisticsItem in baselineStatisticsItems)
            {
                foreach (var nonBaselineStatisticsItem in nonBaselineStatisticsItems.Where(i => string.Equals(i.Key, baselineStatisticsItem.Key)))
                {
                    var statisticsComparisonItem = CreateBenchmarkStatisticsComparisonItem(baselineStatisticsItem, nonBaselineStatisticsItem);

                    statisticsComparison.Add(statisticsComparisonItem);
                }
            }
            report.StatisticsComparison = statisticsComparison;

            return report;
        }
    }
}