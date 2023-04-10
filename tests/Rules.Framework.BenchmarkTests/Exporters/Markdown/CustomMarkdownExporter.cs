namespace Rules.Framework.BenchmarkTests.Exporters.Markdown
{
    using System.Linq;
    using System.Text;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Reports;

    internal class CustomMarkdownExporter : CustomExporterBase
    {
        public static IExporter Default => new CustomMarkdownExporter();

        protected override string FileExtension => "md";

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            var report = this.CreateReport(summary);

            string markdownReport = CustomMarkdownExporter.ParseReportAsMarkdown(report);

            logger.WriteLine(markdownReport);
        }

        private static string ParseReportAsMarkdown(Common.BenchmarkReport benchmarkReport)
        {
            var hostEnvInfo = benchmarkReport.Environment!;
            var stringBuilder = new StringBuilder("# Benchmark Results Report")
                .AppendLine()
                .AppendLine($"Date & Time: {benchmarkReport.Date:yyyy-MM-dd HH:mm:ss}")
                .AppendLine()
                .AppendLine("## Environment")
                .AppendLine()
                .AppendLine($">{hostEnvInfo.BenchmarkDotNetCaption} Version={hostEnvInfo.BenchmarkDotNetVersion}")
                .AppendLine(">")
                .AppendLine($">Processor={hostEnvInfo.ProcessorName},{hostEnvInfo.PhysicalCoreCount} physical cores, {hostEnvInfo.LogicalCoreCount} logical cores")
                .AppendLine(">")
                .AppendLine($">Architecture={hostEnvInfo.Architecture}, Runtime={hostEnvInfo.DotNetRuntimeVersion}, Configuration={hostEnvInfo.BuildConfiguration}")
                .AppendLine(">")
                .AppendLine($">.NET CLI Version={hostEnvInfo.DotNetCliVersion}");

            stringBuilder.AppendLine()
                .AppendLine("## Statistics")
            .AppendLine();

            var benchmarkStatisticsItems = benchmarkReport.Statistics!;
            var hasBranchInstructionsPerOp = benchmarkStatisticsItems.FirstOrDefault()?.BranchInstructionsPerOp is not null;
            var hasBranchMispredictionsPerOp = benchmarkStatisticsItems.FirstOrDefault()?.BranchMispredictionsPerOp is not null;
            var hasGen0Collects = benchmarkStatisticsItems.FirstOrDefault()?.Gen0Collects is not null;
            var hasAllocatedMemory = benchmarkStatisticsItems.FirstOrDefault()?.AllocatedMemory is not null;

            stringBuilder.Append("|Name|Parameters|Mean Time Taken|Std Error|")
                .AppendIf(() => "Branch<br/>Instructions/Op|", () => hasBranchInstructionsPerOp)
                .AppendIf(() => "Branch<br/>Mispredictions/Op|", () => hasBranchMispredictionsPerOp)
                .AppendIf(() => "GC Gen0|", () => hasGen0Collects)
                .AppendIf(() => "Allocated Memory|", () => hasAllocatedMemory)
                .AppendLine()
                .Append("|---|---|---|---|")
                .AppendIf(() => "---|", () => hasBranchInstructionsPerOp)
                .AppendIf(() => "---|", () => hasBranchMispredictionsPerOp)
                .AppendIf(() => "---|", () => hasGen0Collects)
                .AppendIf(() => "---|", () => hasAllocatedMemory)
                .AppendLine();

            foreach (var statisticsItem in benchmarkStatisticsItems!)
            {
                stringBuilder.Append($"|{statisticsItem.Key}")
                    .Append($"|{statisticsItem.Parameters}")
                    .Append($"|{statisticsItem.MeanTimeTaken!.Value.ToString((string)statisticsItem.MeanTimeTaken.Format)} {statisticsItem.MeanTimeTaken.Unit}")
                    .Append($"|{statisticsItem.StandardError!.Value.ToString((string)statisticsItem.StandardError.Format)} {statisticsItem.StandardError.Unit}")
                    .AppendIf(() => $"|{statisticsItem.BranchInstructionsPerOp!.Value.ToString((string)statisticsItem.BranchInstructionsPerOp.Format)}", () => hasBranchInstructionsPerOp)
                    .AppendIf(() => $"|{statisticsItem.BranchMispredictionsPerOp!.Value.ToString((string)statisticsItem.BranchMispredictionsPerOp.Format)}", () => hasBranchMispredictionsPerOp)
                    .AppendIf(() => $"|{statisticsItem.Gen0Collects!.Value.ToString((string)statisticsItem.Gen0Collects.Format)}", () => hasGen0Collects)
                    .AppendIf(() => $"|{statisticsItem.AllocatedMemory!.Value.ToString((string)statisticsItem.AllocatedMemory.Format)} {statisticsItem.AllocatedMemory.Unit}", () => hasAllocatedMemory)
                    .AppendLine("|");
            }

            stringBuilder.AppendLine()
                .AppendLine("## Statistics Comparison")
                .AppendLine()
                .Append("|Name|Baseline|Compare|Mean Time Taken<br/>[Baseline]|Mean Time Taken<br/>[Compare]|Mean Time Taken<br/>[Comparison %]")
                .AppendIf(() => "|Allocated Memory<br/>[Baseline]|Allocated Memory<br/>[Compare]|Allocated Memory<br/>[Comparison %]", () => hasAllocatedMemory)
                .AppendLine("|")
                .Append("|---|---|---|---|---|---")
                .AppendIf(() => "|---|---|---", () => hasAllocatedMemory)
                .AppendLine("|");

            foreach (var statisticsComparisonItem in benchmarkReport.StatisticsComparison!)
            {
                stringBuilder.Append($"|{statisticsComparisonItem.Key}")
                    .Append($"|{statisticsComparisonItem.BaselineParameters}")
                    .Append($"|{statisticsComparisonItem.CompareParameters}")
                    .Append($"|{statisticsComparisonItem.BaselineMeanTimeTaken!.Value.ToString((string)statisticsComparisonItem.BaselineMeanTimeTaken.Format)} {statisticsComparisonItem.BaselineMeanTimeTaken.Unit}")
                    .Append($"|{statisticsComparisonItem.CompareMeanTimeTaken!.Value.ToString((string)statisticsComparisonItem.CompareMeanTimeTaken.Format)} {statisticsComparisonItem.CompareMeanTimeTaken.Unit}")
                    .Append($"|{statisticsComparisonItem.MeanTimeTakenCompareRate!.Value.ToString((string)statisticsComparisonItem.MeanTimeTakenCompareRate.Format)} {statisticsComparisonItem.MeanTimeTakenCompareRate.Unit}")
                    .AppendIf(() => $"|{statisticsComparisonItem.BaselineAllocatedMemory!.Value.ToString((string)statisticsComparisonItem.BaselineAllocatedMemory.Format)} {statisticsComparisonItem.BaselineAllocatedMemory.Unit}", () => hasAllocatedMemory)
                    .AppendIf(() => $"|{statisticsComparisonItem.CompareAllocatedMemory!.Value.ToString((string)statisticsComparisonItem.CompareAllocatedMemory.Format)} {statisticsComparisonItem.CompareAllocatedMemory.Unit}", () => hasAllocatedMemory)
                    .AppendIf(() => $"|{statisticsComparisonItem.AllocatedMemoryRate!.Value.ToString((string)statisticsComparisonItem.AllocatedMemoryRate.Format)} {statisticsComparisonItem.AllocatedMemoryRate.Unit}", () => hasAllocatedMemory)
                    .AppendLine("|");
            }

            return stringBuilder.ToString();
        }
    }
}