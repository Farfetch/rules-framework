namespace Rules.Framework.Providers.InMemory.IntegrationTests.Scenarios.Scenario2
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario2;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class CarInsuranceAdvisorTests : BaseScenarioTests
    {
        private readonly IInMemoryRulesStorage<ContentTypes, ConditionTypes> inMemoryRulesStorage;

        public CarInsuranceAdvisorTests()
        {
            this.inMemoryRulesStorage = new InMemoryRulesStorage<ContentTypes, ConditionTypes>();
            this.LoadInMemoryStorage<ContentTypes, ConditionTypes, CarInsuranceAdvices>(
                DataSourceFilePath,
                this.inMemoryRulesStorage,
                (c) => this.Parse<CarInsuranceAdvices>((string)c));
        }

        private static string DataSourceFilePath => $@"{Environment.CurrentDirectory}/Scenarios/Scenario2/rules-framework-tests.car-insurance-advisor.json";

        [Fact]
        public async Task GetCarInsuranceAdvice_RepairCostsNotWorthIt_ReturnsPayOldCar()
        {
            // Arrange
            var expected = CarInsuranceAdvices.PayOldCar;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2016, 06, 01, 20, 23, 23);
            var expectedConditions = new[]
            {
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCosts,
                    Value = 0.0m
                },
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCostsCommercialValueRate,
                    Value = 0.0m
                }
            };

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(opt =>
                {
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();

            // Act
            var actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            var actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Fact]
        public async Task GetCarInsuranceAdvice_RepairCostsNotWorthIt_ReturnsRefusePaymentPerFranchise()
        {
            // Arrange
            var expected = CarInsuranceAdvices.RefusePaymentPerFranchise;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
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

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(opt =>
                {
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();

            // Act
            var actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            var actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Fact]
        public async Task GetCarInsuranceAdvice_UpdatesRuleAndAddsNewOneAndEvaluates_ReturnsPay()
        {
            // Arrange
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new[]
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

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(opt =>
                {
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();

            var rulesDataSource = CreateRulesDataSourceTest<ContentTypes, ConditionTypes>(this.inMemoryRulesStorage);

            var ruleBuilderResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Car Insurance Advise on self damage coverage")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentTypes.CarInsuranceAdvice, CarInsuranceAdvices.Pay)
                .Build();

            var existentRules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            var ruleToAdd = ruleBuilderResult.Rule;
            var ruleToUpdate1 = existentRules1.FirstOrDefault();
            ruleToUpdate1.Priority = 2;

            // Act 1
            var updateOperationResult1 = await rulesEngine.UpdateRuleAsync(ruleToUpdate1);

            var eval1 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            var rules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>());

            // Assert 1
            updateOperationResult1.Should().NotBeNull();
            updateOperationResult1.IsSuccess.Should().BeTrue();

            eval1.Priority.Should().Be(2);
            var content1 = eval1.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content1.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            var rule11 = rules1.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule11.Should().NotBeNull();
            rule11.Priority.Should().Be(1);
            var rule12 = rules1.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule12.Should().NotBeNull();
            rule12.Priority.Should().Be(2);
            var rule13 = rules1.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule13.Should().NotBeNull();
            rule13.Priority.Should().Be(3);

            // Act 2
            var addOperationResult = await rulesEngine.AddRuleAsync(ruleToAdd, new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtRuleName,
                AtRuleNameOptionValue = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            var eval2 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            var rules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>());

            // Assert 2
            addOperationResult.Should().NotBeNull();
            addOperationResult.IsSuccess.Should().BeTrue();

            eval2.Priority.Should().Be(3);
            CarInsuranceAdvices content2 = eval2.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content2.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            var rule21 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule21.Should().NotBeNull();
            rule21.Priority.Should().Be(1);
            var rule22 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule22.Should().NotBeNull();
            rule22.Priority.Should().Be(2);
            var rule23 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule23.Should().NotBeNull();
            rule23.Priority.Should().Be(3);
            var rule24 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule24.Should().NotBeNull();
            rule24.Priority.Should().Be(4);

            // Act 3
            var existentRules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });
            var ruleToUpdate2 = existentRules2.FirstOrDefault();
            ruleToUpdate2.Priority = 4;

            var updateOperationResult2 = await rulesEngine.UpdateRuleAsync(ruleToUpdate2);

            var eval3 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            var rules3 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>());

            // Assert 3
            updateOperationResult2.Should().NotBeNull();
            updateOperationResult2.IsSuccess.Should().BeTrue();

            eval3.Priority.Should().Be(4);
            var content3 = eval3.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content3.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            var rule31 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule31.Should().NotBeNull();
            rule31.Priority.Should().Be(1);
            var rule32 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule32.Should().NotBeNull();
            rule32.Priority.Should().Be(2);
            var rule33 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule33.Should().NotBeNull();
            rule33.Priority.Should().Be(3);
            var rule34 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule34.Should().NotBeNull();
            rule34.Priority.Should().Be(4);
        }
    }
}