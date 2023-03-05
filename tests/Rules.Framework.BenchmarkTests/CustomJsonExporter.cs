namespace Rules.Framework.BenchmarkTests
{
    using System.Collections.Generic;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Exporters.Json;
    using BenchmarkDotNet.Reports;

    internal class CustomJsonExporter : JsonExporterBase
    {
        public CustomJsonExporter(string fileNameSuffix = "", bool indentJson = false, bool excludeMeasurements = false)
            : base(indentJson, excludeMeasurements)
        {
            FileNameSuffix = fileNameSuffix;
        }

        public static IExporter Default => FullCompressed;

        public static IExporter Full => new CustomJsonExporter("-full", indentJson: true);

        public static IExporter FullCompressed => new CustomJsonExporter("-full-compressed");

        protected override string FileNameSuffix { get; } = string.Empty;

        protected override IReadOnlyDictionary<string, object> GetDataToSerialize(Summary summary)
        {
            var dataToSerialize = base.GetDataToSerialize(summary);

            var baselineClassifierColumn = summary.Table.Columns.First(x => x.OriginalColumn.ColumnName == "Baseline");

            var benchmarksDataToSerialize = ((IEnumerable<IReadOnlyDictionary<string, object>>)dataToSerialize["Benchmarks"]).ToArray();

            var newBenchmarks = new List<IReadOnlyDictionary<string, object>>();
            for (int i = 0; i < benchmarksDataToSerialize.Length; i++)
            {
                var benchmarkData = (Dictionary<string, object>)benchmarksDataToSerialize[i];
                benchmarkData["Baseline"] = baselineClassifierColumn.Content[i];
                newBenchmarks.Add(benchmarkData);
            }

            var newDataToSerialize = dataToSerialize.ToDictionary(x => x.Key, x => x.Value);

            newDataToSerialize["Benchmarks"] = newBenchmarks;

            return newDataToSerialize;
        }
    }
}