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
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario2;
    using Rules.Framework.Providers.MongoDb;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Providers.MongoDb.IntegrationTests;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Rules.Framework.Serialization;
    using Xunit;

    public sealed class CarInsuranceAdvisorTests : IDisposable
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public CarInsuranceAdvisorTests()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();

            Stream? rulesFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Rules.Framework.Providers.MongoDb.IntegrationTests.Scenarios.Scenario2.rules-framework-tests.car-insurance-advisor.json");

            IEnumerable<RuleDataModel> rules;
            using (StreamReader streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                string json = streamReader.ReadToEnd();

                IEnumerable<RuleDataModel> array = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                rules = array.Select(t =>
                {
                    t.Content = t.Content as string;

                    return t;
                }).ToList();
            }

            IMongoDatabase mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
            IMongoCollection<RuleDataModel> mongoCollection = mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            mongoCollection.InsertMany(rules);
        }

        public void Dispose()
        {
            IMongoDatabase mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
        }

        [Fact]
        public async Task GetCarInsuranceAdvice_RepairCostsNotWorthIt_ReturnsRefusePaymentPerFranchise()
        {
            // Arrange
            CarInsuranceAdvices expected = CarInsuranceAdvices.RefusePaymentPerFranchise;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[]
            {
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCosts,
                    Value = 800.00000m
                },
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCostsCommercialValueRate,
                    Value = 23.45602m
                }
            };

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(reo =>
                {
                    reo.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions).ConfigureAwait(false);

            // Assert
            actual.Should().NotBeNull();
            CarInsuranceAdvices actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Fact]
        public async Task GetCarInsuranceAdvice_UpdatesRuleAndAddsNewOneAndEvaluates_ReturnsPay()
        {
            // Arrange
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[]
            {
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCosts,
                    Value = 800.00000m
                },
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCostsCommercialValueRate,
                    Value = 23.45602m
                }
            };

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(reo =>
                {
                    reo.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = CreateRulesDataSourceTest<ContentTypes, ConditionTypes>(this.mongoClient, this.mongoDbProviderSettings);

            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Car Insurance Advise on self damage coverage")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.CarInsuranceAdvice, (t) => CarInsuranceAdvices.Pay))
                .Build();
            IEnumerable<Rule<ContentTypes, ConditionTypes>> existentRules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            }).ConfigureAwait(false);

            Rule<ContentTypes, ConditionTypes> ruleToAdd = ruleBuilderResult.Rule;
            Rule<ContentTypes, ConditionTypes> ruleToUpdate1 = existentRules1.First();
            ruleToUpdate1.Priority = 2;

            // Act 1
            RuleOperationResult updateOperationResult1 = await rulesEngine.UpdateRuleAsync(ruleToUpdate1).ConfigureAwait(false);

            Rule<ContentTypes, ConditionTypes> eval1 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions).ConfigureAwait(false);

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);

            // Assert 1
            updateOperationResult1.Should().NotBeNull();
            updateOperationResult1.IsSuccess.Should().BeTrue();

            eval1.Priority.Should().Be(2);
            CarInsuranceAdvices content1 = eval1.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content1.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            Rule<ContentTypes, ConditionTypes> rule11 = rules1.First(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule11.Should().NotBeNull();
            rule11.Priority.Should().Be(1);
            Rule<ContentTypes, ConditionTypes> rule12 = rules1.First(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule12.Should().NotBeNull();
            rule12.Priority.Should().Be(2);
            Rule<ContentTypes, ConditionTypes> rule13 = rules1.First(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule13.Should().NotBeNull();
            rule13.Priority.Should().Be(3);

            // Act 2
            RuleOperationResult addOperationResult = await rulesEngine.AddRuleAsync(ruleToAdd, new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtRuleName,
                AtRuleNameOptionValue = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            Rule<ContentTypes, ConditionTypes> eval2 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions).ConfigureAwait(false);

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);

            // Assert 2
            addOperationResult.Should().NotBeNull();
            addOperationResult.IsSuccess.Should().BeTrue();

            eval2.Priority.Should().Be(3);
            CarInsuranceAdvices content2 = eval2.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content2.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            Rule<ContentTypes, ConditionTypes> rule21 = rules2.First(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule21.Should().NotBeNull();
            rule21.Priority.Should().Be(1);
            Rule<ContentTypes, ConditionTypes> rule22 = rules2.First(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule22.Should().NotBeNull();
            rule22.Priority.Should().Be(2);
            Rule<ContentTypes, ConditionTypes> rule23 = rules2.First(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule23.Should().NotBeNull();
            rule23.Priority.Should().Be(3);
            Rule<ContentTypes, ConditionTypes> rule24 = rules2.First(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule24.Should().NotBeNull();
            rule24.Priority.Should().Be(4);

            // Act 3
            IEnumerable<Rule<ContentTypes, ConditionTypes>> existentRules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            }).ConfigureAwait(false);
            Rule<ContentTypes, ConditionTypes> ruleToUpdate2 = existentRules2.First();
            ruleToUpdate2.Priority = 4;

            RuleOperationResult updateOperationResult2 = await rulesEngine.UpdateRuleAsync(ruleToUpdate2).ConfigureAwait(false);

            Rule<ContentTypes, ConditionTypes> eval3 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions).ConfigureAwait(false);

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules3 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);

            // Assert 3
            updateOperationResult2.Should().NotBeNull();
            updateOperationResult2.IsSuccess.Should().BeTrue();

            eval3.Priority.Should().Be(4);
            CarInsuranceAdvices content3 = eval3.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content3.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            Rule<ContentTypes, ConditionTypes> rule31 = rules3.First(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule31.Should().NotBeNull();
            rule31.Priority.Should().Be(1);
            Rule<ContentTypes, ConditionTypes> rule32 = rules3.First(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule32.Should().NotBeNull();
            rule32.Priority.Should().Be(2);
            Rule<ContentTypes, ConditionTypes> rule33 = rules3.First(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule33.Should().NotBeNull();
            rule33.Priority.Should().Be(3);
            Rule<ContentTypes, ConditionTypes> rule34 = rules3.First(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule34.Should().NotBeNull();
            rule34.Priority.Should().Be(4);
        }

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

        private static IRulesDataSource<TContentType, TConditionType> CreateRulesDataSourceTest<TContentType, TConditionType>(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings)
        {
            IContentSerializationProvider<TContentType> contentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider<TContentType>();
            IRuleFactory<TContentType, TConditionType> ruleFactory = new RuleFactory<TContentType, TConditionType>(contentSerializationProvider);
            return new MongoDbProviderRulesDataSource<TContentType, TConditionType>(
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