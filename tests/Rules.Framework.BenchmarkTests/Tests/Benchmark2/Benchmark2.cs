namespace Rules.Framework.BenchmarkTests.Tests.Benchmark2
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Rules.Framework.Generic;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark2 : IBenchmark
    {
        private readonly Scenario7Data benchmarkData = new Scenario7Data();
        private IRulesEngine<ContentTypes, ConditionTypes>? genericRulesEngine;

        [ParamsAllValues]
        public bool EnableCompilation { get; set; }

        [Params("in-memory", "mongo-db")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.genericRulesEngine!.MatchOneAsync(ContentTypes.Songs, this.benchmarkData.MatchDate, this.benchmarkData.Conditions).ConfigureAwait(false);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark2))
                .Configure(options =>
                {
                    options.EnableCompilation = this.EnableCompilation;
                })
                .Build();

            await rulesEngine.CreateContentTypeAsync(nameof(ContentTypes.Songs));

            foreach (var rule in this.benchmarkData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }

            this.genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();
        }

        [GlobalCleanup]
        public async Task TearDownAsync()
        {
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark2)).ConfigureAwait(false);
            this.genericRulesEngine = null;
        }
    }
}