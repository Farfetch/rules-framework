namespace Rules.Framework.InMemory.Sample.Engine
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class RulesEngineProvider
    {
        private readonly Lazy<Task<IRulesEngine>> lazyRulesEngine;

        public RulesEngineProvider(RulesBuilder rulesBuilder)
        {
            lazyRulesEngine = new Lazy<Task<IRulesEngine>>(async () =>
            {
                var rulesEngine = RulesEngineBuilder
                    .CreateRulesEngine()
                    .SetInMemoryDataSource()
                    .Configure(opt => opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins)
                    .Build();

                await rulesBuilder.BuildAsync(rulesEngine).ConfigureAwait(false);

                return rulesEngine;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Task<IRulesEngine> GetRulesEngineAsync()
            => lazyRulesEngine.Value;
    }
}