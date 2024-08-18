namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Events;
    using Newtonsoft.Json;
    using Rules.Framework;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario2;
    using Rules.Framework.Providers.MongoDb;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Providers.MongoDb.IntegrationTests;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Xunit;

    public sealed class CarInsuranceAdvisorTests : IDisposable
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public CarInsuranceAdvisorTests()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();

            var rulesFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario2.rules-framework-tests.car-insurance-advisor.json");

            IEnumerable<RuleDataModel> rules;
            using (var streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                var json = streamReader.ReadToEnd();

                var array = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                rules = array.Select(t =>
                {
                    t.Content = t.Content as string;

                    return t;
                }).ToList();
            }

            var contentTypes = rules
                .Select(r => new ContentTypeDataModel
                {
                    Creation = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    Name = r.ContentType,
                })
                .Distinct()
                .ToArray();

            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.ContentTypesCollectionName);
            var contentTypesMongoCollection = mongoDatabase.GetCollection<ContentTypeDataModel>(this.mongoDbProviderSettings.ContentTypesCollectionName);
            contentTypesMongoCollection.InsertMany(contentTypes);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
            var rulesMongoCollection = mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);
            rulesMongoCollection.InsertMany(rules);
        }

        public void Dispose()
        {
            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_RepairCostsNotWorthIt_ReturnsRefusePaymentPerFranchise(bool enableCompilation)
        {
            // Arrange
            var expected = CarInsuranceAdvices.RefusePaymentPerFranchise;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
            {
                new Condition<ConditionTypes>(ConditionTypes.RepairCosts, 800.00000m),
                new Condition<ConditionTypes>(ConditionTypes.RepairCostsCommercialValueRate, 23.45602m)
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();

            // Act
            var actual = await genericRulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            var actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_UpdatesRuleAndAddsNewOneAndEvaluates_ReturnsPay(bool enableCompilation)
        {
            // Arrange
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
            {
                new Condition<ConditionTypes>(ConditionTypes.RepairCosts, 800.00000m),
                new Condition<ConditionTypes>(ConditionTypes.RepairCostsCommercialValueRate, 23.45602m)
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt =>
                {
                    opt.EnableCompilation = enableCompilation;
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();

            var rulesDataSource = CreateRulesDataSourceTest<ContentTypes, ConditionTypes>(this.mongoClient, this.mongoDbProviderSettings);

            var ruleBuilderResult = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Car Insurance Advise on self damage coverage")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentTypes.CarInsuranceAdvice, CarInsuranceAdvices.Pay)
                .Build();
            var existentRules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            var ruleToAdd = ruleBuilderResult.Rule;
            var ruleToUpdate1 = existentRules1.First();
            ruleToUpdate1.Priority = 2;

            // Act 1
            var updateOperationResult1 = await rulesEngine.UpdateRuleAsync(ruleToUpdate1);

            var eval1 = await genericRulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            var rules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());

            // Assert 1
            updateOperationResult1.Should().NotBeNull();
            updateOperationResult1.IsSuccess.Should().BeTrue();

            eval1.Priority.Should().Be(2);
            var content1 = eval1.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content1.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            var rule11 = rules1.First(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule11.Should().NotBeNull();
            rule11.Priority.Should().Be(1);
            var rule12 = rules1.First(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule12.Should().NotBeNull();
            rule12.Priority.Should().Be(2);
            var rule13 = rules1.First(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule13.Should().NotBeNull();
            rule13.Priority.Should().Be(3);

            // Act 2
            var addOperationResult = await rulesEngine.AddRuleAsync(ruleToAdd, new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtRuleName,
                AtRuleNameOptionValue = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            var eval2 = await genericRulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            var rules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());

            // Assert 2
            addOperationResult.Should().NotBeNull();
            addOperationResult.IsSuccess.Should().BeTrue();

            eval2.Priority.Should().Be(3);
            var content2 = eval2.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content2.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            var rule21 = rules2.First(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule21.Should().NotBeNull();
            rule21.Priority.Should().Be(1);
            var rule22 = rules2.First(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule22.Should().NotBeNull();
            rule22.Priority.Should().Be(2);
            var rule23 = rules2.First(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule23.Should().NotBeNull();
            rule23.Priority.Should().Be(3);
            var rule24 = rules2.First(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule24.Should().NotBeNull();
            rule24.Priority.Should().Be(4);

            // Act 3
            var existentRules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });
            var ruleToUpdate2 = existentRules2.First();
            ruleToUpdate2.Priority = 4;

            var updateOperationResult2 = await rulesEngine.UpdateRuleAsync(ruleToUpdate2);

            var eval3 = await genericRulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            var rules3 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());

            // Assert 3
            updateOperationResult2.Should().NotBeNull();
            updateOperationResult2.IsSuccess.Should().BeTrue();

            eval3.Priority.Should().Be(4);
            var content3 = eval3.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content3.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            var rule31 = rules3.First(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule31.Should().NotBeNull();
            rule31.Priority.Should().Be(1);
            var rule32 = rules3.First(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule32.Should().NotBeNull();
            rule32.Priority.Should().Be(2);
            var rule33 = rules3.First(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule33.Should().NotBeNull();
            rule33.Priority.Should().Be(3);
            var rule34 = rules3.First(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule34.Should().NotBeNull();
            rule34.Priority.Should().Be(4);
        }

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

        private static IRulesDataSource CreateRulesDataSourceTest<TContentType, TConditionType>(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings)
        {
            var contentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider();
            var ruleFactory = new RuleFactory(contentSerializationProvider);
            return new MongoDbProviderRulesDataSource(
                    mongoClient,
                    mongoDbProviderSettings,
                    ruleFactory);
        }

        private MongoDbProviderSettings CreateProviderSettings() => new MongoDbProviderSettings
        {
            DatabaseName = "rules-framework-tests",
            RulesCollectionName = "car-insurance-advisor"
        };
    }
}