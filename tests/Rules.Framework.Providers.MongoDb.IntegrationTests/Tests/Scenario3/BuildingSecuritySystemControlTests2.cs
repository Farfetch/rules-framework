namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Tests.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Driver;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario3;
    using Rules.Framework.Providers.MongoDb;
    using Xunit;

    public sealed class BuildingSecuritySystemControlTests2 : IDisposable
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public BuildingSecuritySystemControlTests2()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();
        }

        [Fact]
        public async Task create()
        {
            // Assert
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Build();

            var rule = RuleBuilder
                .NewRule<SecuritySystemActionables, SecuritySystemConditions>()
                .WithName("Top5")
                .WithDateBegin(new DateTime(2021, 01, 01))
                .WithContentContainer(new ContentContainer<SecuritySystemActionables>(SecuritySystemActionables.PowerSystem, t => "80"))
                .WithCondition(cnb => cnb.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(x1 =>
                            x1.AsValued(SecuritySystemConditions.TemperatureCelsius)
                                .OfDataType<IEnumerable<decimal>>()
                                .WithComparisonOperator(Operators.In)
                                .SetOperand(new[] { 100m, 200m, 300m })
                                .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(SecuritySystemConditions.SmokeRate)
                            .OfDataType<IEnumerable<int>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { 12, 16, 24, 36 })
                            .Build())
                        .Build())
                .Build();

            // Act

            var result = await rulesEngine.AddRuleAsync(rule.Rule, RuleAddPriorityOption.AtTop);

            var test = await rulesEngine.SearchAsync(
                new SearchArgs<SecuritySystemActionables, SecuritySystemConditions>(SecuritySystemActionables.PowerSystem, DateTime.UtcNow, DateTime.MaxValue));

            // Assert
            result.Should().NotBeNull();
            test.Should().NotBeNull();
        }

        public void Dispose()
        {
            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
        }

        private static MongoClient CreateMongoClient() => new($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");

        private static MongoDbProviderSettings CreateProviderSettings() => new MongoDbProviderSettings
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "security-system-actionables"
        };
    }
}