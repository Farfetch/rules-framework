namespace Rules.Framework.Rql.IntegrationTests.Scenarios.Scenario8
{
    using System;
    using Rules.Framework.IntegrationTests.Common.Scenarios;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8;

    public class RulesEngineWithScenario8RulesFixture : IDisposable
    {
        public RulesEngineWithScenario8RulesFixture()
        {
            this.RulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource()
                .Configure(options =>
                {
                    options.EnableCompilation = true;
                })
                .Build();

            var scenarioData = new Scenario8Data();

            ScenarioLoader.LoadScenarioAsync(this.RulesEngine, scenarioData).GetAwaiter().GetResult();
        }

        public IRulesEngine<ContentTypes, ConditionTypes> RulesEngine { get; private set; }

        public void Dispose()
        {
            if (this.RulesEngine != null)
            {
                this.RulesEngine = null!;
            }

            GC.SuppressFinalize(this);
        }
    }
}