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
    using Rules.Framework;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario3;
    using Rules.Framework.Providers.MongoDb;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Xunit;

    public sealed class BuildingSecuritySystemControlTests : IDisposable
    {
        private readonly MongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public BuildingSecuritySystemControlTests()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();

            var rulesFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario3.rules-framework-tests.security-system-actionables.json");

            IEnumerable<RuleDataModel> rules;
            using (var streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                var json = streamReader.ReadToEnd();

                var array = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                rules = array!.Select(t =>
                {
                    SecuritySystemAction securitySystemAction = t.Content.ToObject<SecuritySystemAction>();
                    dynamic dynamicContent = new ExpandoObject();
                    dynamicContent.ActionId = securitySystemAction.ActionId;
                    dynamicContent.ActionName = securitySystemAction.ActionName;
                    t.Content = dynamicContent;

                    return t;
                }).ToList();
            }

            var contentTypes = rules
                .Select(r => new RulesetDataModel
                {
                    Creation = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    Name = r.Ruleset,
                })
                .Distinct()
                .ToArray();

            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesetsCollectionName);
            var contentTypesMongoCollection = mongoDatabase.GetCollection<RulesetDataModel>(this.mongoDbProviderSettings.RulesetsCollectionName);
            contentTypesMongoCollection.InsertMany(contentTypes);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
            var rulesMongoCollection = mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);
            rulesMongoCollection.InsertMany(rules);
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
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var newRuleResult = Rule.Create<SecuritySystemActionables, SecuritySystemConditions>("Activate ventilation system rule")
                .InRuleset(SecuritySystemActionables.FireSystem)
                .SetContent(new SecuritySystemAction
                {
                    ActionId = new Guid("ef0d65ae-ec76-492a-84db-5cb9090c3eaa"),
                    ActionName = "ActivateVentilationSystem"
                })
                .Since(new DateTime(2018, 01, 01))
                .ApplyWhen(b => b.Value(SecuritySystemConditions.SmokeRate, Operators.GreaterThanOrEqual, 30.0m))
                .Build();
            var newRule = newRuleResult.Rule;

            _ = await genericRulesEngine.AddRuleAsync(newRule, RuleAddPriorityOption.AtBottom);

            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

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
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

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
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

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
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesetsCollectionName);
        }

        private static MongoClient CreateMongoClient() => new($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");

        private static MongoDbProviderSettings CreateProviderSettings() => new()
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "security-system-actionables",
        };
    }
}