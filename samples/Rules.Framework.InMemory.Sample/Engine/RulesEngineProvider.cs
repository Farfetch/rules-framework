namespace Rules.Framework.InMemory.Sample.Engine
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Rules.Framework.InMemory.Sample.Enums;
    using global::Rules.Framework.Providers.InMemory;

    internal class RulesEngineProvider
    {
        private readonly Lazy<Task<RulesEngine<ContentTypes, ConditionTypes>>> lazyRulesEngine;

        public RulesEngineProvider(RulesBuilder rulesBuilder)
        {
            lazyRulesEngine = new Lazy<Task<RulesEngine<ContentTypes, ConditionTypes>>>(async () =>
            {
                var rulesEngine = RulesEngineBuilder
                    .CreateRulesEngine()
                    .WithContentType<ContentTypes>()
                    .WithConditionType<ConditionTypes>()
                    .SetInMemoryDataSource()
                    .Configure(opt => opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins)
                    .Build();

                await rulesBuilder.BuildAsync(rulesEngine).ConfigureAwait(false);

                return rulesEngine;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Task<RulesEngine<ContentTypes, ConditionTypes>> GetRulesEngineAsync()
            => lazyRulesEngine.Value;
    }
}