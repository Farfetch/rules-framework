namespace Rules.Framework.Admin.Dashboard.Sample.Engine
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Rules.Framework.Admin.Dashboard.Sample.Enums;
    using global::Rules.Framework.Providers.InMemory;

    public class RulesEngineProvider
    {
        private readonly Lazy<Task<IRulesEngine<ContentTypes, ConditionTypes>>> lazyRulesEngine;

        public RulesEngineProvider(RulesBuilder rulesBuilder)
        {
            lazyRulesEngine = new Lazy<Task<IRulesEngine<ContentTypes, ConditionTypes>>>(async () =>
            {
                var rulesEngine = RulesEngineBuilder
                    .CreateRulesEngine()
                    .WithContentType<ContentTypes>()
                    .WithConditionType<ConditionTypes>()
                    .SetInMemoryDataSource()
                    .Configure(c => c.PriotityCriteria = PriorityCriterias.BottommostRuleWins)
                    .Build();

                await rulesBuilder.BuildAsync(rulesEngine).ConfigureAwait(false);

                return rulesEngine;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Task<IRulesEngine<ContentTypes, ConditionTypes>> GetRulesEngineAsync()
            => lazyRulesEngine.Value;
    }
}