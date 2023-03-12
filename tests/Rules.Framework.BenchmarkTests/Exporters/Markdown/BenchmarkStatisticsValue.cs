namespace Rules.Framework.BenchmarkTests.Exporters.Markdown
{
    internal class BenchmarkStatisticsValue
    {
        public string Format { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public decimal Value { get; set; }
    }
}