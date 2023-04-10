namespace Rules.Framework.BenchmarkTests.Exporters.Json
{
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Reports;
    using Newtonsoft.Json;

    internal class CustomJsonExporter : CustomExporterBase
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
            var report = this.CreateReport(summary);

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
    }
}