namespace Rules.Framework.BenchmarkTests.Exporters.Markdown
{
    internal class Environment
    {
        public string Architecture { get; set; } = string.Empty;

        public string BenchmarkDotNetCaption { get; set; } = string.Empty;

        public string BenchmarkDotNetVersion { get; set; } = string.Empty;

        public string BuildConfiguration { get; set; } = string.Empty;

        public string DotNetCliVersion { get; set; } = string.Empty;

        public string DotNetRuntimeVersion { get; set; } = string.Empty;

        public int LogicalCoreCount { get; set; }

        public int PhysicalCoreCount { get; set; }

        public string ProcessorName { get; set; } = string.Empty;
    }
}