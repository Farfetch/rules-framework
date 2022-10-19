namespace Rules.Framework.BenchmarkTests.Tests.Benchmark2
{
    using BenchmarkDotNet.Attributes;
    using Rules.Framework.Core;
    using System.Linq;
    using System.Threading.Tasks;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark2 : IBenchmark
    {
        private readonly Benchmark2Data benchmarkData = new Benchmark2Data();
        private RulesEngine<ContentTypes, ConditionTypes> rulesEngine;

        [ParamsAllValues]
        public bool EnableCompilation { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.rulesEngine.MatchOneAsync(ContentTypes.Songs, this.benchmarkData.MatchDate, this.benchmarkData.Conditions).ConfigureAwait(false);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            this.rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(new InMemoryRulesDataSource<ContentTypes, ConditionTypes>(Enumerable.Empty<Rule<ContentTypes, ConditionTypes>>()))
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
            this.rulesEngine = null;
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
