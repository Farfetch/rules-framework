namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark3 : IBenchmark
    {
        private readonly Benchmark3Data benchmarkData = new Benchmark3Data();
        private RulesEngine<ContentTypes, ConditionTypes>? rulesEngine;

        [ParamsAllValues]
        public bool EnableCompilation { get; set; }

        [Params("in-memory", "mongo-db")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.rulesEngine!.MatchOneAsync(ContentTypes.TexasHoldemPokerSingleCombinations, this.benchmarkData.MatchDate, this.benchmarkData.Conditions).ConfigureAwait(false);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            this.rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark1))
                .Configure(options =>
                {
                    options.EnableCompilation = this.EnableCompilation;
                })
                .Build();

            foreach (var rule in this.benchmarkData.Rules)
            {
                await this.rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);
            }
        }

        [GlobalCleanup]
        public async Task TearDownAsync()
        {
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark1)).ConfigureAwait(false);
            this.rulesEngine = null;
        }
    }
}