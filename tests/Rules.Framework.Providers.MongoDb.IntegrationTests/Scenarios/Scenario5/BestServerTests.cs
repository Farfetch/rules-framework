namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Driver;
    using Rules.Framework;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario5;
    using Rules.Framework.Providers.MongoDb;
    using Xunit;

    public sealed class BestServerTests : IDisposable
    {
        public static readonly IEnumerable<object[]> DataTest = new List<object[]>
        {
            new object[]
            {
                new[]
                {
                    new Condition<BestServerConditions>(BestServerConditions.Price, 100),
                    new Condition<BestServerConditions>(BestServerConditions.Memory, 12),
                    new Condition<BestServerConditions>(BestServerConditions.StoragePartionable, true),
                    new Condition<BestServerConditions>(BestServerConditions.Brand, "AMD")
                },
                "Best Server Top5"
            },
            new object[]
            {
                new[]
                {
                    new Condition<BestServerConditions>(BestServerConditions.Price, 110),
                    new Condition<BestServerConditions>(BestServerConditions.Memory, 12),
                    new Condition<BestServerConditions>(BestServerConditions.StoragePartionable, true),
                    new Condition<BestServerConditions>(BestServerConditions.Brand, "AMD")
                },
                "Best Server Default"
            }
        }.SelectMany(x => new[] { false, true }.Select(c => new object[] { x[0], x[1], c }));

        private readonly MongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public BestServerTests()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();
        }

        [Theory]
        [MemberData(nameof(DataTest))]
        public async Task BestServer_InEvaluation(IEnumerable<Condition<BestServerConditions>> conditions, string expectedRuleName, bool enableCompilation)
        {
            // Arrange
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<BestServerConfigurations, BestServerConditions>();
            await genericRulesEngine.CreateRulesetAsync(BestServerConfigurations.BestServerEvaluation);

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Top5")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Top5")
                .Since(DateTime.Parse("2021-05-29Z"))
                .Until(DateTime.Parse("2021-05-31Z"))
                .ApplyWhen(c => c
                    .And(a => a
                        .Value(BestServerConditions.Price, Operators.In, new[] { 100m, 200m, 300m })
                        .Value(BestServerConditions.Memory, Operators.In, new[] { 12, 16, 24, 36 })
                        .Value(BestServerConditions.Memory, Operators.NotIn, new[] { 4, 8 })
                        .Value(BestServerConditions.StoragePartionable, Operators.In, new[] { true })
                        .Value(BestServerConditions.Brand, Operators.In, new[] { "AMD", "Intel", "Cisco" })
                    )
                )
                .Build();

            // Act 2 - Create rule default
            var ruleBuilderResultDefault = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Default")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Default")
                .Since(DateTime.Parse("2021-05-29Z"))
                .Until(DateTime.Parse("2021-05-31Z"))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Assert 3
            ruleBuilderResultDefault.Should().NotBeNull();
            errors = ruleBuilderResultDefault.Errors.Any() ? ruleBuilderResultDefault.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResultDefault.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule default: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            await rulesEngine.AddRuleAsync(ruleBuilderResultDefault.Rule, RuleAddPriorityOption.ByPriorityNumber(2));
            await rulesEngine.AddRuleAsync(ruleBuilderResult.Rule, RuleAddPriorityOption.ByPriorityNumber(1));

            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");

            var actual = await genericRulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Name.Should().BeEquivalentTo(expectedRuleName);
        }

        public void Dispose()
        {
            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
        }

        private static MongoClient CreateMongoClient() => new($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");

        private static MongoDbProviderSettings CreateProviderSettings() => new()
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "best-server",
        };
    }
}