namespace Rules.Framework.Providers.InMemory.IntegrationTests.Tests.Scenario2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario2;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class CarInsuranceAdvisorTests : BaseScenarioTests
    {
        private readonly InMemoryRulesStorage<ContentTypes, ConditionTypes> inMemoryRulesStorage;

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

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottomMostRuleWins;
                })
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            CarInsuranceAdvices actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Fact]
        public async Task GetCarInsuranceAdvice_RepairCostsNotWorthIt_ReturnsPayOldCar()
        {
            // Arrange
            CarInsuranceAdvices expected = CarInsuranceAdvices.PayOldCar;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2016, 06, 01, 20, 23, 23);
            Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[]
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

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottomMostRuleWins;
                })
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

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

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton(this.inMemoryRulesStorage);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottomMostRuleWins;
                })
                .Build();

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = CreateRulesDataSourceTest<ContentTypes, ConditionTypes>(this.inMemoryRulesStorage);

            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Car Insurance Advise on self damage coverage")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithPriority(1)
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.CarInsuranceAdvice, (t) => CarInsuranceAdvices.Pay))
                .Build();
            IEnumerable<Rule<ContentTypes, ConditionTypes>> existentRules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            Rule<ContentTypes, ConditionTypes> ruleToAdd = ruleBuilderResult.Rule;
            Rule<ContentTypes, ConditionTypes> ruleToUpdate1 = existentRules1.FirstOrDefault();
            ruleToUpdate1.Priority = 2;

            // Act 1
            RuleOperationResult updateOperationResult1 = await rulesEngine.UpdateRuleAsync(ruleToUpdate1);

            Rule<ContentTypes, ConditionTypes> eval1 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules1 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>());

            // Assert 1
            updateOperationResult1.Should().NotBeNull();
            updateOperationResult1.IsSuccess.Should().BeTrue();

            eval1.Priority.Should().Be(2);
            CarInsuranceAdvices content1 = eval1.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content1.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            Rule<ContentTypes, ConditionTypes> rule11 = rules1.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule11.Should().NotBeNull();
            rule11.Priority.Should().Be(1);
            Rule<ContentTypes, ConditionTypes> rule12 = rules1.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule12.Should().NotBeNull();
            rule12.Priority.Should().Be(2);
            Rule<ContentTypes, ConditionTypes> rule13 = rules1.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule13.Should().NotBeNull();
            rule13.Priority.Should().Be(3);

            // Act 2
            RuleOperationResult addOperationResult = await rulesEngine.AddRuleAsync(ruleToAdd, new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtRuleName,
                AtRuleNameOptionValue = "Car Insurance Advise on repair costs lower than franchise boundary"
            });

            Rule<ContentTypes, ConditionTypes> eval2 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>());

            // Assert 2
            addOperationResult.Should().NotBeNull();
            addOperationResult.IsSuccess.Should().BeTrue();

            eval2.Priority.Should().Be(3);
            CarInsuranceAdvices content2 = eval2.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content2.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            Rule<ContentTypes, ConditionTypes> rule21 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule21.Should().NotBeNull();
            rule21.Priority.Should().Be(1);
            Rule<ContentTypes, ConditionTypes> rule22 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule22.Should().NotBeNull();
            rule22.Priority.Should().Be(2);
            Rule<ContentTypes, ConditionTypes> rule23 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule23.Should().NotBeNull();
            rule23.Priority.Should().Be(3);
            Rule<ContentTypes, ConditionTypes> rule24 = rules2.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule24.Should().NotBeNull();
            rule24.Priority.Should().Be(4);

            // Act 3
            IEnumerable<Rule<ContentTypes, ConditionTypes>> existentRules2 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>
            {
                Name = "Car Insurance Advise on repair costs lower than franchise boundary"
            });
            Rule<ContentTypes, ConditionTypes> ruleToUpdate2 = existentRules2.FirstOrDefault();
            ruleToUpdate2.Priority = 4;

            RuleOperationResult updateOperationResult2 = await rulesEngine.UpdateRuleAsync(ruleToUpdate2);

            Rule<ContentTypes, ConditionTypes> eval3 = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules3 = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>());

            // Assert 3
            updateOperationResult2.Should().NotBeNull();
            updateOperationResult2.IsSuccess.Should().BeTrue();

            eval3.Priority.Should().Be(4);
            CarInsuranceAdvices content3 = eval3.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            content3.Should().Be(CarInsuranceAdvices.RefusePaymentPerFranchise);

            Rule<ContentTypes, ConditionTypes> rule31 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
            rule31.Should().NotBeNull();
            rule31.Priority.Should().Be(1);
            Rule<ContentTypes, ConditionTypes> rule32 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on self damage coverage");
            rule32.Should().NotBeNull();
            rule32.Priority.Should().Be(2);
            Rule<ContentTypes, ConditionTypes> rule33 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            rule33.Should().NotBeNull();
            rule33.Priority.Should().Be(3);
            Rule<ContentTypes, ConditionTypes> rule34 = rules3.FirstOrDefault(r => r.Name == "Car Insurance Advise on repair costs lower than franchise boundary");
            rule34.Should().NotBeNull();
            rule34.Priority.Should().Be(4);
        }
    }
}