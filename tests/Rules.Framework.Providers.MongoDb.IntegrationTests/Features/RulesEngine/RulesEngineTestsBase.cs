namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Features.RulesEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Events;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;

    public abstract class RulesEngineTestsBase : IDisposable
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly ContentType TestContentType;

        protected RulesEngineTestsBase(ContentType testContentType)
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();
            this.TestContentType = testContentType;

            this.RulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .WithContentType<ContentType>()
                .WithConditionType<ConditionType>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();
        }

        protected RulesEngine<ContentType, ConditionType> RulesEngine { get; }

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
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        protected async Task<Rule<ContentType, ConditionType>> MatchOneAsync(
            DateTime matchDate,
            Condition<ConditionType>[] conditions) => await RulesEngine.MatchOneAsync(
                TestContentType,
                matchDate,
                conditions)
            .ConfigureAwait(false);

        private static MongoClient CreateMongoClient()
        {
            MongoClientSettings settings = MongoClientSettings.FromConnectionString($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");
            settings.ClusterConfigurator = (cb) =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    Trace.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            return new MongoClient(settings);
        }

        private MongoDbProviderSettings CreateProviderSettings() => new MongoDbProviderSettings
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "features-tests"
        };
    }
}