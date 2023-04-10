namespace Rules.Framework.BenchmarkTests.Exporters.Common
{
    internal class BenchmarkStatisticsValue
    {
        private BenchmarkStatisticsValue(string format, string unit, decimal value)
        {
            this.Format = format;
            this.Unit = unit;
            this.Value = value;
        }

        public string Format { get; set; }

        public string Unit { get; set; }

        public decimal Value { get; set; }

        public static BenchmarkStatisticsValue New(string format, string unit, decimal value)
            => new BenchmarkStatisticsValue(format, unit, value);
    }
}