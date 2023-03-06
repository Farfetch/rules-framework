namespace Rules.Framework.BenchmarkTests.Exporters.Json
{
    using System.Collections.Generic;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Reports;
    using Newtonsoft.Json;

    internal class CustomJsonExporter : ExporterBase
    {
        public CustomJsonExporter(bool indentJson)
        {
            this.IndentJson = indentJson;
        }

        public static IExporter Compressed => new CustomJsonExporter(indentJson: false);

        public static IExporter Default => Indented;

        public static IExporter Indented => new CustomJsonExporter(indentJson: true);

        protected override string FileExtension => "json";

        private bool IndentJson { get; }

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            var report = CustomJsonExporter.CreateReport(summary);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None
            };

            if (this.IndentJson)
            {
                settings.Formatting = Formatting.Indented;
            }

            string jsonText = JsonConvert.SerializeObject(report, settings);

            logger.WriteLine(jsonText);
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
            var allocatedMemoryMetric = benchmarkReport.Metrics.First(m => m.Key == "Allocated Memory");
            var branchInstructionsMetric = benchmarkReport.Metrics.First(m => m.Key == "BranchInstructions");
            var branchMispredictionsMetric = benchmarkReport.Metrics.First(m => m.Key == "BranchMispredictions");
            var gen0CollectsMetric = benchmarkReport.Metrics.First(m => m.Key == "Gen0Collects");

            var statisticsItem = new BenchmarkStatisticsItem
            {
                AllocatedMemory = new BenchmarkStatisticsValue
                {
                    Format = allocatedMemoryMetric.Value.Descriptor.NumberFormat,
                    Unit = allocatedMemoryMetric.Value.Descriptor.Unit,
                    Value = Convert.ToDecimal(allocatedMemoryMetric.Value.Value),
                },
                Baseline = baselineClassifierColumn.Content[current++],
                BranchInstructionsPerOp = new BenchmarkStatisticsValue
                {
                    Format = branchInstructionsMetric.Value.Descriptor.NumberFormat,
                    Unit = branchInstructionsMetric.Value.Descriptor.Unit,
                    Value = Convert.ToDecimal(branchInstructionsMetric.Value.Value),
                },
                BranchMispredictionsPerOp = new BenchmarkStatisticsValue
                {
                    Format = branchMispredictionsMetric.Value.Descriptor.NumberFormat,
                    Unit = branchMispredictionsMetric.Value.Descriptor.Unit,
                    Value = Convert.ToDecimal(branchMispredictionsMetric.Value.Value)
                },
                Gen0Collects = new BenchmarkStatisticsValue
                {
                    Format = gen0CollectsMetric.Value.Descriptor.NumberFormat,
                    Unit = gen0CollectsMetric.Value.Descriptor.Unit,
                    Value = Convert.ToDecimal(gen0CollectsMetric.Value.Value)
                },
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
    }
}