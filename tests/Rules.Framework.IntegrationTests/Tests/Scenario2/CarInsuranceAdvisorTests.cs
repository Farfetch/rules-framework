namespace Rules.Framework.IntegrationTests.Tests.Scenario2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario2;
    using Xunit;

    public class CarInsuranceAdvisorTests
    {
        private static string DataSourceFilePath => $@"{Environment.CurrentDirectory}/Scenarios/Scenario2/rules-framework-tests.car-insurance-advisor.json";

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_ClaimDescriptionContionsAlcoholOrDrugs_ReturnsPerformInvestigation(bool enableCompilation)
        {
            // Arrange
            CarInsuranceAdvices expected = CarInsuranceAdvices.PerformInvestigation;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2020, 06, 01);
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
                },
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.ClaimDescription,
                    Value = "Driver A claims that Driver B appeared to be under the effect of alcohol."
                }
            };

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath, serializedContent: false);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottommostRuleWins;
                    reo.EnableCompilation = enableCompilation;
                })
                .Build();

            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Car Insurance Advise on on accident under the effect of drugs or alcohol")
                .WithDateBegin(DateTime.Parse("2020-01-01"))
                .WithCondition(b =>
                {
                    return b.AsComposed()
                        .WithLogicalOperator(LogicalOperators.Or)
                        .AddCondition(cb =>
                        {
                            return cb.AsValued(ConditionTypes.ClaimDescription)
                                .OfDataType<string>()
                                .WithComparisonOperator(Operators.Contains)
                                .SetOperand("alcohol")
                                .Build();
                        })
                        .AddCondition(cb =>
                        {
                            return cb.AsValued(ConditionTypes.ClaimDescription)
                                .OfDataType<string>()
                                .WithComparisonOperator(Operators.Contains)
                                .SetOperand("drugs")
                                .Build();
                        })
                        .Build();
                })
                .WithContentContainer(new ContentContainer<ContentTypes>(expectedContent, t => CarInsuranceAdvices.PerformInvestigation))
                .Build();

            // Act
            await rulesEngine.AddRuleAsync(ruleBuilderResult.Rule, RuleAddPriorityOption.AtBottom).ConfigureAwait(false);

            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions).ConfigureAwait(false);

            // Assert
            actual.Should().NotBeNull();
            CarInsuranceAdvices actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_RepairCostsNotWorthIt_ReturnsRefusePaymentPerFranchise(bool enableCompilation)
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

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath, serializedContent: false);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottommostRuleWins;
                    reo.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            CarInsuranceAdvices actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_SearchForRulesExcludingRulesWithoutSearchConditions_ReturnsNoRules(bool enableCompilation)
        {
            // Arrange
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            SearchArgs<ContentTypes, ConditionTypes> searchArgs = new SearchArgs<ContentTypes, ConditionTypes>(expectedContent, expectedMatchDate, expectedMatchDate)
            {
                Conditions = new Condition<ConditionTypes>[]
                {
                    new Condition<ConditionTypes>
                    {
                        Type = ConditionTypes.RepairCosts,
                        Value = 800.00000m
                    },
                    new Condition<ConditionTypes>
                    {
                        Type = ConditionTypes.RepairCostsCommercialValueRate,
                        Value = 86.33m
                    }
                },
                ExcludeRulesWithoutSearchConditions = true
            };

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath, serializedContent: false);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottommostRuleWins;
                    reo.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            IEnumerable<Rule<ContentTypes, ConditionTypes>> actual = await rulesEngine.SearchAsync(searchArgs);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().HaveCount(0);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_SearchForRulesWithRepairCostsGreaterThan1000_Returns2Rules(bool enableCompilation)
        {
            // Arrange
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            SearchArgs<ContentTypes, ConditionTypes> searchArgs = new SearchArgs<ContentTypes, ConditionTypes>(expectedContent, expectedMatchDate, expectedMatchDate)
            {
                Conditions = new Condition<ConditionTypes>[]
                {
                    new Condition<ConditionTypes>
                    {
                        Type = ConditionTypes.RepairCosts,
                        Value = 1200.00000m
                    }
                },
                ExcludeRulesWithoutSearchConditions = false
            };

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath, serializedContent: false);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottommostRuleWins;
                    reo.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            IEnumerable<Rule<ContentTypes, ConditionTypes>> actual = await rulesEngine.SearchAsync(searchArgs);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().HaveCount(2);
            actual.Should().Contain(r => r.Name == "Car Insurance Advise on repair costs greater than 80% of commercial value");
            actual.Should().Contain(r => r.Name == "Car Insurance Advise on repair costs lesser than 80% of commercial value");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCarInsuranceAdvice_UpdatesRuleAndAddsNewOneAndEvaluates_ReturnsPay(bool enableCompilation)
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

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath, serializedContent: false);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottommostRuleWins;
                    reo.EnableCompilation = enableCompilation;
                })
                .Build();

            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Car Insurance Advise on self damage coverage")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
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