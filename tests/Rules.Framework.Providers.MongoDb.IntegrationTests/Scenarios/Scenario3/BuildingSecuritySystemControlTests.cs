namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario3;
    using Rules.Framework.Providers.MongoDb;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Xunit;

    public sealed class BuildingSecuritySystemControlTests : IDisposable
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public BuildingSecuritySystemControlTests()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();

            Stream? rulesFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario3.rules-framework-tests.security-system-actionables.json");

            IEnumerable<RuleDataModel> rules;
            using (var streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                string json = streamReader.ReadToEnd();

                var array = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                rules = array.Select(t =>
                {
                    SecuritySystemAction securitySystemAction = t.Content.ToObject<SecuritySystemAction>();
                    dynamic dynamicContent = new ExpandoObject();
                    dynamicContent.ActionId = securitySystemAction.ActionId;
                    dynamicContent.ActionName = securitySystemAction.ActionName;
                    t.Content = dynamicContent;

                    return t;
                }).ToList();
            }

            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
            var mongoCollection = mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            mongoCollection.InsertMany(rules);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BuildingSecuritySystem_FireScenario_ReturnsActionsToTrigger(bool enableCompilation)
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.FireSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
            {
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.TemperatureCelsius, 100.0m),
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.SmokeRate, 55.0m),
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.PowerStatus, "Online")
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            var newRuleResult = RuleBuilder.NewRule<SecuritySystemActionables, SecuritySystemConditions>()
                .WithName("Activate ventilation system rule")
                .WithDateBegin(new DateTime(2018, 01, 01))
                .WithContent(SecuritySystemActionables.FireSystem, new SecuritySystemAction
                {
                    ActionId = new Guid("ef0d65ae-ec76-492a-84db-5cb9090c3eaa"),
                    ActionName = "ActivateVentilationSystem"
                })
                .WithCondition(b => b.Value(SecuritySystemConditions.SmokeRate, Core.Operators.GreaterThanOrEqual, 30.0m))
                .Build();
            var newRule = newRuleResult.Rule;

            _ = await rulesEngine.AddRuleAsync(newRule, RuleAddPriorityOption.AtBottom);

            var actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            var securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "CallFireBrigade")
                .And.Contain(ssa => ssa.ActionName == "CallPolice")
                .And.Contain(ssa => ssa.ActionName == "ActivateSprinklers")
                .And.Contain(ssa => ssa.ActionName == "ActivateVentilationSystem")
                .And.HaveCount(4);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BuildingSecuritySystem_PowerFailureScenario_ReturnsActionsToTrigger(bool enableCompilation)
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
            {
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.TemperatureCelsius, 100.0m),
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.SmokeRate, 55.0m),
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.PowerStatus, "Offline")
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            var actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            var securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.Contain(ssa => ssa.ActionName == "EnableEmergencyPower")
                .And.Contain(ssa => ssa.ActionName == "CallPowerGridPicket");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BuildingSecuritySystem_PowerShutdownScenario_ReturnsActionsToTrigger(bool enableCompilation)
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
            {
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.TemperatureCelsius, 100.0m),
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.SmokeRate, 55.0m),
                new Condition<SecuritySystemConditions>(SecuritySystemConditions.PowerStatus, "Shutdown")
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<SecuritySystemActionables>()
                .WithConditionType<SecuritySystemConditions>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            var actual = await rulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            var securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.HaveCount(1);
        }

        public void Dispose()
        {
            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
        }

        private static MongoClient CreateMongoClient() => new MongoClient($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");

        private static MongoDbProviderSettings CreateProviderSettings() => new MongoDbProviderSettings
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "security-system-actionables"
        };
    }
}