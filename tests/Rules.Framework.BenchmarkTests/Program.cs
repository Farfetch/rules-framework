using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using McMaster.Extensions.CommandLineUtils;
using Rules.Framework.BenchmarkTests;
using Rules.Framework.BenchmarkTests.Exporters.Markdown;

[assembly: SimpleJob(RuntimeMoniker.Net60)]

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
            }

        manualConfig.AddExporter(HtmlExporter.Default);
            manualConfig.AddExporter(CustomMarkdownExporter.Default);
            manualConfig.WithOption(ConfigOptions.JoinSummary, true);

            var artifactsPath = artifactsPathOption.Value();
            if (artifactsPath is not null or "")
            {
                manualConfig.WithArtifactsPath(artifactsPath);
            }

            var column = new CustomBaselineClassifierColumn(
                bc => bc.Parameters.Items.Any(p => p.Name == "EnableCompilation" && (bool)p.Value == false));
            manualConfig.AddColumn(column);

        _ = BenchmarkRunner.Run(typeof(Program).Assembly, manualConfig);
        });

        return app.Execute(args);
    }
}