namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Features.RulesEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Events;
    using Rules.Framework.Generic;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;

    public abstract class RulesEngineTestsBase : IDisposable
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly RulesetNames TestRuleset;

        protected RulesEngineTestsBase(RulesetNames testRuleset)
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();
            this.TestRuleset = testRuleset;

            var rulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();

            this.RulesEngine = rulesEngine.MakeGeneric<RulesetNames, ConditionNames>();
            this.RulesEngine.CreateRulesetAsync(testRuleset).GetAwaiter().GetResult();
        }

        protected IRulesEngine<RulesetNames, ConditionNames> RulesEngine { get; }

        public void Dispose()
        {
            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
        }

        protected void AddRules(IEnumerable<RuleSpecification> ruleSpecifications)
        {
            foreach (var ruleSpecification in ruleSpecifications)
            {
                this.RulesEngine.AddRuleAsync(
                    ruleSpecification.Rule,
                    ruleSpecification.RuleAddPriorityOption)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        protected async Task<Rule<RulesetNames, ConditionNames>> MatchOneAsync(
            DateTime matchDate,
            Dictionary<ConditionNames, object> conditions) => await RulesEngine.MatchOneAsync(
                TestRuleset,
                matchDate,
                conditions);

        private static MongoClient CreateMongoClient()
        {
            var settings = MongoClientSettings.FromConnectionString($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");
            settings.ClusterConfigurator = (cb) =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    Trace.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            return new MongoClient(settings);
        }

        private static MongoDbProviderSettings CreateProviderSettings() => new MongoDbProviderSettings
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "features-tests"
        };
    }
}