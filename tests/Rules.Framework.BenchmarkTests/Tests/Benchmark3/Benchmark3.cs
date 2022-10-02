namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using BenchmarkDotNet.Attributes;
    using Rules.Framework.Core;
    using System.Linq;
    using System.Threading.Tasks;

    public class Benchmark3 : IBenchmark
    {
        private readonly Benchmark3Data benchmarkData = new Benchmark3Data();
        private RulesEngine<ContentTypes, ConditionTypes> rulesEngine;

        [Benchmark]
        public async Task RunAsync()
        {
            await this.rulesEngine.MatchOneAsync(ContentTypes.TexasHoldemPokerSingleCombinations, this.benchmarkData.MatchDate, this.benchmarkData.Conditions).ConfigureAwait(false);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            this.rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(new InMemoryRulesDataSource<ContentTypes, ConditionTypes>(Enumerable.Empty<Rule<ContentTypes, ConditionTypes>>()))
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
