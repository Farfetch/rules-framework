using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

[assembly: SimpleJob(RuntimeMoniker.Net60)]

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting benchmark tests.");
        Console.WriteLine();

        var manualConfig = ManualConfig.CreateMinimumViable();
        manualConfig.AddDiagnoser(MemoryDiagnoser.Default);
        manualConfig.AddHardwareCounters(HardwareCounter.BranchInstructions, HardwareCounter.BranchMispredictions);
        manualConfig.AddExporter(HtmlExporter.Default);

        _ = BenchmarkRunner.Run(typeof(Program).Assembly, manualConfig);

        Console.WriteLine("Press any key to exit...");
        Console.Read();
    }
}