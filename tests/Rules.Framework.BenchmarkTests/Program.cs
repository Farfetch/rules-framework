using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using McMaster.Extensions.CommandLineUtils;

[assembly: SimpleJob(RuntimeMoniker.Net80)]

internal static class Program
{
    private static int Main(string[] args)
    {
        var app = new CommandLineApplication();

        app.HelpOption();

        var artifactsPathOption = app.Option("-a|--artifacts-path <ARTIFACTS_PATH>", "Sets the artifacts path", CommandOptionType.SingleValue, config =>
        {
            config.DefaultValue = "artifacts";
        });

        app.OnExecute(() =>
        {
            Console.WriteLine("Starting benchmark tests.");
            Console.WriteLine();

            var manualConfig = ManualConfig.CreateMinimumViable();
            manualConfig.AddDiagnoser(MemoryDiagnoser.Default);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                manualConfig.AddHardwareCounters(HardwareCounter.BranchInstructions, HardwareCounter.BranchMispredictions);
                var etwProfilerConfig = new EtwProfilerConfig(cpuSampleIntervalInMilliseconds: 0.125f, performExtraBenchmarksRun: false);
                manualConfig.AddDiagnoser(new EtwProfiler(etwProfilerConfig));
            }

            manualConfig.AddExporter(HtmlExporter.Default);
            manualConfig.WithOption(ConfigOptions.JoinSummary, true);

            var artifactsPath = artifactsPathOption.Value();
            if (artifactsPath is not null or "")
            {
                manualConfig.WithArtifactsPath(artifactsPath);
            }

            _ = BenchmarkRunner.Run(typeof(Program).Assembly, manualConfig);
        });

        return app.Execute(args);
    }
}