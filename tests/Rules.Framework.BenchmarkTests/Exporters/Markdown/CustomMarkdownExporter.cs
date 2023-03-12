namespace Rules.Framework.BenchmarkTests.Exporters.Markdown
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Reports;

    internal class CustomMarkdownExporter : ExporterBase
    {
        public CustomMarkdownExporter()
        {
        }

        public static IExporter Default => new CustomMarkdownExporter();

        protected override string FileExtension => "md";

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            var report = CustomMarkdownExporter.CreateReport(summary);

            string markdownReport = CustomMarkdownExporter.ParseReportAsMarkdown(report);

            logger.WriteLine(markdownReport);
        }

        private static decimal CalculateRate(decimal baselineValue, decimal compareValue)
            => ((baselineValue - compareValue) / baselineValue) * 100;

        private static BenchmarkStatisticsComparisonItem CreateBenchmarkStatisticsComparisonItem(BenchmarkStatisticsItem? baselineStatisticsItem, BenchmarkStatisticsItem? nonBaselineStatisticsItem) => new BenchmarkStatisticsComparisonItem
        {
            AllocatedMemoryRate = new BenchmarkStatisticsValue
            {
                Format = "0.##",
                Unit = "%",
                Value = CalculateRate(
                                        baselineStatisticsItem.AllocatedMemory?.Value ?? 0,
                                        nonBaselineStatisticsItem.AllocatedMemory?.Value ?? 0),
            },
            BaselineAllocatedMemory = baselineStatisticsItem.AllocatedMemory,
            BaselineMeanTimeTaken = baselineStatisticsItem.MeanTimeTaken,
            BaselineParameters = baselineStatisticsItem.Parameters,
            CompareAllocatedMemory = nonBaselineStatisticsItem.AllocatedMemory,
            CompareMeanTimeTaken = nonBaselineStatisticsItem.MeanTimeTaken,
            CompareParameters = nonBaselineStatisticsItem.Parameters,
            Key = baselineStatisticsItem.Key,
            MeanTimeTakenCompareRate = new BenchmarkStatisticsValue
            {
                Format = "0.##",
                Unit = "%",
                Value = CalculateRate(
                                        baselineStatisticsItem.MeanTimeTaken?.Value ?? 0,
                                        nonBaselineStatisticsItem.MeanTimeTaken?.Value ?? 0),
            }
        };

        private static BenchmarkStatisticsItem CreateBenchmarkStatisticsItem(SummaryTable.SummaryTableColumn baselineClassifierColumn, ref int current, BenchmarkDotNet.Reports.BenchmarkReport benchmarkReport)
        {
            var allocatedMemoryMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "Allocated Memory");
            var branchInstructionsMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "BranchInstructions");
            var branchMispredictionsMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "BranchMispredictions");
            var gen0CollectsMetric = benchmarkReport.Metrics.FirstOrDefault(m => m.Key == "Gen0Collects");

            var statisticsItem = new BenchmarkStatisticsItem
            {
                AllocatedMemory = allocatedMemoryMetric.Key is null ? null : CreateBenchmarkStatisticsValue(allocatedMemoryMetric),
                Baseline = baselineClassifierColumn.Content[current++],
                BranchInstructionsPerOp =
                    branchInstructionsMetric.Key is null ? null : CreateBenchmarkStatisticsValue(branchInstructionsMetric),
                BranchMispredictionsPerOp =
                    branchMispredictionsMetric.Key is null ? null : CreateBenchmarkStatisticsValue(branchMispredictionsMetric),
                Gen0Collects = gen0CollectsMetric.Key is null ? null : CreateBenchmarkStatisticsValue(gen0CollectsMetric),
                Key = $"{benchmarkReport.BenchmarkCase.Descriptor.Type.Name}.{benchmarkReport.BenchmarkCase.Descriptor.WorkloadMethod.Name}",
                MeanTimeTaken = new BenchmarkStatisticsValue
                {
                    Format = "N0",
                    Unit = "ns",
                    Value = Convert.ToDecimal(benchmarkReport.ResultStatistics?.Mean ?? 0),
                },
                Parameters = benchmarkReport.BenchmarkCase.Parameters.DisplayInfo,
                StandardError = new BenchmarkStatisticsValue
                {
                    Format = "N0",
                    Unit = "ns",
                    Value = Convert.ToDecimal(benchmarkReport.ResultStatistics?.StandardError ?? 0),
                },
            };
            return statisticsItem;
        }

        private static BenchmarkStatisticsValue CreateBenchmarkStatisticsValue(KeyValuePair<string, Metric> allocatedMemoryMetric) => new BenchmarkStatisticsValue
        {
            Format = allocatedMemoryMetric.Value.Descriptor.NumberFormat,
            Unit = allocatedMemoryMetric.Value.Descriptor.Unit,
            Value = Convert.ToDecimal(allocatedMemoryMetric.Value.Value),
        };

        private static Environment CreateEnvironment(Summary summary) => new Environment
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

        private static BenchmarkReport CreateReport(Summary summary)
        {
            var report = new BenchmarkReport
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

        private static string ParseReportAsMarkdown(BenchmarkReport benchmarkReport)
        {
            var hostEnvInfo = benchmarkReport.Environment;
            var stringBuilder = new StringBuilder("# Benchmark Results Report")
                .AppendLine()
                .AppendLine($"Date & Time: {benchmarkReport.Date:yyyy-MM-dd HH:mm:ss}")
                .AppendLine()
                .AppendLine("## Environment")
                .AppendLine()
                .AppendLine($">{hostEnvInfo.BenchmarkDotNetCaption} Version={benchmarkReport.Environment.BenchmarkDotNetVersion}")
                .AppendLine(">")
                .AppendLine($">Processor={hostEnvInfo.ProcessorName},{hostEnvInfo.PhysicalCoreCount} physical cores, {hostEnvInfo.LogicalCoreCount} logical cores")
                .AppendLine(">")
                .AppendLine($">Architecture={hostEnvInfo.Architecture}, Runtime={hostEnvInfo.DotNetRuntimeVersion}, Configuration={hostEnvInfo.BuildConfiguration}")
                .AppendLine(">")
                .AppendLine($">.NET CLI Version={hostEnvInfo.DotNetCliVersion}");

            stringBuilder.AppendLine()
                .AppendLine("## Statistics")
            .AppendLine();

            var hasBranchInstructionsPerOp = benchmarkReport.Statistics.FirstOrDefault()?.BranchInstructionsPerOp is not null;
            var hasBranchMispredictionsPerOp = benchmarkReport.Statistics.FirstOrDefault()?.BranchMispredictionsPerOp is not null;
            var hasGen0Collects = benchmarkReport.Statistics.FirstOrDefault()?.Gen0Collects is not null;
            var hasAllocatedMemory = benchmarkReport.Statistics.FirstOrDefault()?.AllocatedMemory is not null;

            stringBuilder.Append("|Name|Parameters|Mean Time Taken|Std Error|")
                .AppendIf("Branch<br/>Instructions/Op|", () => hasBranchInstructionsPerOp)
                .AppendIf("Branch<br/>Mispredictions/Op|", () => hasBranchMispredictionsPerOp)
                .AppendIf("GC Gen0|", () => hasGen0Collects)
                .AppendIf("Allocated Memory|", () => hasAllocatedMemory)
                .AppendLine()
                .Append("|---|---|---|---|")
                .AppendIf("---|", () => hasBranchInstructionsPerOp)
                .AppendIf("---|", () => hasBranchMispredictionsPerOp)
                .AppendIf("---|", () => hasGen0Collects)
                .AppendIf("---|", () => hasAllocatedMemory)
                .AppendLine();

            foreach (var statisticsItem in benchmarkReport.Statistics)
            {
                stringBuilder.Append($"|{statisticsItem.Key}")
                    .Append($"|{statisticsItem.Parameters}")
                    .Append($"|{statisticsItem.MeanTimeTaken.Value.ToString((string)statisticsItem.MeanTimeTaken.Format)} {statisticsItem.MeanTimeTaken.Unit}")
                    .Append($"|{statisticsItem.StandardError.Value.ToString((string)statisticsItem.StandardError.Format)} {statisticsItem.StandardError.Unit}")
                    .AppendIf($"|{statisticsItem.BranchInstructionsPerOp.Value.ToString((string)statisticsItem.BranchInstructionsPerOp.Format)}", () => hasBranchInstructionsPerOp)
                    .AppendIf($"|{statisticsItem.BranchMispredictionsPerOp.Value.ToString((string)statisticsItem.BranchMispredictionsPerOp.Format)}", () => hasBranchMispredictionsPerOp)
                    .AppendIf($"|{statisticsItem.Gen0Collects.Value.ToString((string)statisticsItem.Gen0Collects.Format)}", () => hasGen0Collects)
                    .AppendIf($"|{statisticsItem.AllocatedMemory.Value.ToString((string)statisticsItem.AllocatedMemory.Format)} {statisticsItem.AllocatedMemory.Unit}", () => hasAllocatedMemory)
                    .AppendLine("|");
            }

            stringBuilder.AppendLine()
                .AppendLine("## Statistics Comparison")
                .AppendLine()
                .Append("|Name|Baseline|Compare|Mean Time Taken<br/>[Baseline]|Mean Time Taken<br/>[Compare]|Mean Time Taken<br/>[Comparison %]")
                .AppendIf("|Allocated Memory<br/>[Baseline]|Allocated Memory<br/>[Compare]|Allocated Memory<br/>[Comparison %]", () => hasAllocatedMemory)
                .AppendLine("|")
                .Append("|---|---|---|---|---|---")
                .AppendIf("|---|---|---", () => hasAllocatedMemory)
                .AppendLine("|");

            foreach (var statisticsComparisonItem in benchmarkReport.StatisticsComparison)
            {
                stringBuilder.Append($"|{statisticsComparisonItem.Key}")
                    .Append($"|{statisticsComparisonItem.BaselineParameters}")
                    .Append($"|{statisticsComparisonItem.CompareParameters}")
                    .Append($"|{statisticsComparisonItem.BaselineMeanTimeTaken.Value.ToString((string)statisticsComparisonItem.BaselineMeanTimeTaken.Format)} {statisticsComparisonItem.BaselineMeanTimeTaken.Unit}")
                    .Append($"|{statisticsComparisonItem.CompareMeanTimeTaken.Value.ToString((string)statisticsComparisonItem.CompareMeanTimeTaken.Format)} {statisticsComparisonItem.CompareMeanTimeTaken.Unit}")
                    .Append($"|{statisticsComparisonItem.MeanTimeTakenCompareRate.Value.ToString((string)statisticsComparisonItem.MeanTimeTakenCompareRate.Format)} {statisticsComparisonItem.MeanTimeTakenCompareRate.Unit}")
                    .AppendIf($"|{statisticsComparisonItem.BaselineAllocatedMemory.Value.ToString((string)statisticsComparisonItem.BaselineAllocatedMemory.Format)} {statisticsComparisonItem.BaselineAllocatedMemory.Unit}", () => hasAllocatedMemory)
                    .AppendIf($"|{statisticsComparisonItem.CompareAllocatedMemory.Value.ToString((string)statisticsComparisonItem.CompareAllocatedMemory.Format)} {statisticsComparisonItem.CompareAllocatedMemory.Unit}", () => hasAllocatedMemory)
                    .AppendIf($"|{statisticsComparisonItem.AllocatedMemoryRate.Value.ToString((string)statisticsComparisonItem.AllocatedMemoryRate.Format)} {statisticsComparisonItem.AllocatedMemoryRate.Unit}", () => hasAllocatedMemory)
                    .AppendLine("|");
            }

            return stringBuilder.ToString();
        }
    }
}