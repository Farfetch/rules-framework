namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Rules.Framework.Generic;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark3 : IBenchmark
    {
        private readonly Scenario8Data benchmarkData = new Scenario8Data();
        private IRulesEngine<PokerRulesets, PokerConditions>? genericRulesEngine;

        [ParamsAllValues]
        public bool EnableCompilation { get; set; }

        [Params("in-memory", "mongo-db")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.genericRulesEngine!.MatchOneAsync(PokerRulesets.TexasHoldemPokerSingleCombinations, this.benchmarkData.MatchDate, this.benchmarkData.Conditions).ConfigureAwait(false);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark3))
                .Configure(options =>
                {
                    options.EnableCompilation = this.EnableCompilation;
                })
                .Build();

            await rulesEngine.CreateRulesetAsync(nameof(PokerRulesets.TexasHoldemPokerSingleCombinations));

            foreach (var rule in this.benchmarkData.Rules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }

            this.genericRulesEngine = rulesEngine.MakeGeneric<PokerRulesets, PokerConditions>();
        }

        [GlobalCleanup]
        public async Task TearDownAsync()
        {
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark3)).ConfigureAwait(false);
            this.genericRulesEngine = null;
        }
    }
}