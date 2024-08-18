namespace Rules.Framework.BenchmarkTests.Tests.Benchmark1
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Rules.Framework.Generic;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark1 : IBenchmark
    {
        private readonly Scenario6Data benchmarkData = new Scenario6Data();
        private IRulesEngine<ContentTypes, ConditionTypes>? rulesEngine;

        [ParamsAllValues]
        public bool EnableCompilation { get; set; }

        [Params("in-memory", "mongo-db")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.rulesEngine!.MatchOneAsync(ContentTypes.ContentType1, this.benchmarkData.MatchDate, this.benchmarkData.Conditions).ConfigureAwait(false);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark1))
                .Configure(options =>
                {
                    options.EnableCompilation = this.EnableCompilation;
                })
                .Build();

            await rulesEngine.CreateContentTypeAsync(nameof(ContentTypes.ContentType1));

            foreach (var rule in this.benchmarkData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);
            }

            this.rulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();
        }

        [GlobalCleanup]
        public async Task TearDownAsync()
        {
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark1)).ConfigureAwait(false);
            this.rulesEngine = null;
        }
    }
}