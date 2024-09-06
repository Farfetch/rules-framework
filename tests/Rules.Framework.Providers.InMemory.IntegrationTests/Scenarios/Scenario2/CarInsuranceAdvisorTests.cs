namespace Rules.Framework.Providers.InMemory.IntegrationTests.Scenarios.Scenario2
{
    using System;
    using System.Collections.Generic;
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
        private readonly IInMemoryRulesStorage inMemoryRulesStorage;

        public CarInsuranceAdvisorTests()
        {
            this.inMemoryRulesStorage = new InMemoryRulesStorage();
            this.LoadInMemoryStorage(
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
            const CarInsuranceRulesetNames expectedRuleset = CarInsuranceRulesetNames.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2016, 06, 01, 20, 23, 23);
            var expectedConditions = new Dictionary<CarInsuranceConditionNames, object>
            {
                { CarInsuranceConditionNames.RepairCosts, 0.0m },
                { CarInsuranceConditionNames.RepairCostsCommercialValueRate, 0.0m },
            };

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(opt =>
                {
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<CarInsuranceRulesetNames, CarInsuranceConditionNames>();

            // Act
            var actual = await genericRulesEngine.MatchOneAsync(expectedRuleset, expectedMatchDate, expectedConditions);

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
            const CarInsuranceRulesetNames expectedRuleset = CarInsuranceRulesetNames.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<CarInsuranceConditionNames, object>
            {
                { CarInsuranceConditionNames.RepairCosts, 800.00000m },
                { CarInsuranceConditionNames.RepairCostsCommercialValueRate, 23.45602m },
            };

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(opt =>
                {
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<CarInsuranceRulesetNames, CarInsuranceConditionNames>();

            // Act
            var actual = await genericRulesEngine.MatchOneAsync(expectedRuleset, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            var actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Fact]
        public async Task GetCarInsuranceAdvice_UpdatesRuleAndAddsNewOneAndEvaluates_ReturnsPay()
        {
            // Arrange
            const CarInsuranceRulesetNames expectedRuleset = CarInsuranceRulesetNames.CarInsuranceAdvice;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<CarInsuranceConditionNames, object>
            {
                { CarInsuranceConditionNames.RepairCosts, 800.00000m },
                { CarInsuranceConditionNames.RepairCostsCommercialValueRate, 23.45602m },
            };

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(opt =>
                {
                    opt.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<CarInsuranceRulesetNames, CarInsuranceConditionNames>();

            var rulesDataSource = CreateRulesDataSourceTest(this.inMemoryRulesStorage);

            var ruleBuilderResult = Rule.Create<CarInsuranceRulesetNames, CarInsuranceConditionNames>("Car Insurance Advise on self damage coverage")
                .InRuleset(expectedRuleset)
                .SetContent(CarInsuranceAdvices.Pay)
                .Since(DateTime.Parse("2018-01-01"))
                .Build();

            var existentRules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            var ruleToAdd = ruleBuilderResult.Rule;
            var ruleToUpdate1 = existentRules1.FirstOrDefault();
            ruleToUpdate1.Priority = 2;

            // Act 1
            var updateOperationResult1 = await rulesEngine.UpdateRuleAsync(ruleToUpdate1);

            var eval1 = await genericRulesEngine.MatchOneAsync(expectedRuleset, expectedMatchDate, expectedConditions);

            var rules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());

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
            var addOperationResult = await genericRulesEngine.AddRuleAsync(ruleToAdd, new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtRuleName,
                AtRuleNameOptionValue = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            var eval2 = await genericRulesEngine.MatchOneAsync(expectedRuleset, expectedMatchDate, expectedConditions);

            var rules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());

            // Assert 2
            addOperationResult.Should().NotBeNull();
            addOperationResult.IsSuccess.Should().BeTrue();

            eval2.Priority.Should().Be(3);
            var content2 = eval2.ContentContainer.GetContentAs<CarInsuranceAdvices>();
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
            var existentRules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });
            var ruleToUpdate2 = existentRules2.FirstOrDefault();
            ruleToUpdate2.Priority = 4;

            var updateOperationResult2 = await rulesEngine.UpdateRuleAsync(ruleToUpdate2);

            var eval3 = await genericRulesEngine.MatchOneAsync(expectedRuleset, expectedMatchDate, expectedConditions);

            var rules3 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());

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