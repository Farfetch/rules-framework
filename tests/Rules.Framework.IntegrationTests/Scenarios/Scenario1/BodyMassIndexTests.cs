namespace Rules.Framework.IntegrationTests.Scenarios.Scenario1
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario1;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    [System.Runtime.InteropServices.Guid("309D98C5-4007-4116-92B1-9FEAD18B9DC3")]
    public class BodyMassIndexTests
    {
        private static string DataSourceFilePath => $@"{Environment.CurrentDirectory}/Scenarios/Scenario1/rules-framework-tests.body-mass-index.datasource.json";

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleFromScratchWithAgeConditionAtPriority1AndNewRuleAtPriority3_NewRuleIsInsertedAndExistentRulePriorityUpdatedAndNewRuleInsertedAfter(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource<ContentTypes, ConditionTypes>(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            var newRuleResult1 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Body Mass Index up to 18 years formula",
                    Value = "weight / ((height + 1) ^ 2)" // Not real, for the sake of the test.
                }))
                .WithCondition(cnb => cnb
                    .AsValued(ConditionTypes.Age)
                    .OfDataType<int>()
                    .WithComparisonOperator(Operators.LesserThanOrEqual)
                    .SetOperand(18)
                    .Build())
                .Build();

            var newRule1 = newRuleResult1.Rule;
            var ruleAddPriorityOption1 = RuleAddPriorityOption.ByPriorityNumber(1);

            var ruleBuilderResult2 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Sample rule")
                .WithDateBegin(DateTime.Parse("2021-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Sample formula",
                    Value = "0"
                }))
                .Build();

            var newRule2 = ruleBuilderResult2.Rule;
            var ruleAddPriorityOption2 = RuleAddPriorityOption.ByPriorityNumber(4);

            // Act
            var ruleOperationResult1 = await rulesEngine.AddRuleAsync(newRule1, ruleAddPriorityOption1).ConfigureAwait(false);
            var ruleOperationResult2 = await rulesEngine.AddRuleAsync(newRule2, ruleAddPriorityOption2).ConfigureAwait(false);

            // Assert
            ruleOperationResult1.Should().NotBeNull();
            ruleOperationResult1.IsSuccess.Should().BeTrue();

            ruleOperationResult2.Should().NotBeNull();
            ruleOperationResult2.IsSuccess.Should().BeTrue();

            var inMemoryRulesStorage = serviceProvider.GetService<IInMemoryRulesStorage<ContentTypes, ConditionTypes>>();
            var rulesDataSource = CreateRulesDataSource(inMemoryRulesStorage);
            var rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
            rules.Should().NotBeNull().And.HaveCount(2);
            rules.Should().ContainEquivalentOf(newRule1);
            newRule1.Priority.Should().Be(1, "rule should to priority 1 if inserted at priority 1");
            newRule2.Priority.Should().Be(2, "rule should have priority 2 if inserted at priority 2, given that last rule after insert was at priority 2.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleWithAgeConditionAtPriority1AndNewRuleAtPriority3_NewRuleIsInsertedAndExistentRulePriorityUpdatedAndNewRuleInsertedAfter(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource<ContentTypes, ConditionTypes>(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(rulesEngine, DataSourceFilePath, typeof(Formula));

            var newRuleResult1 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Body Mass Index up to 18 years formula",
                    Value = "weight / ((height + 1) ^ 2)" // Not real, for the sake of the test.
                }))
                .WithCondition(cnb => cnb
                    .AsValued(ConditionTypes.Age)
                    .OfDataType<int>()
                    .WithComparisonOperator(Operators.LesserThanOrEqual)
                    .SetOperand(18)
                    .Build())
                .Build();

            var newRule1 = newRuleResult1.Rule;
            var ruleAddPriorityOption1 = RuleAddPriorityOption.ByPriorityNumber(1);

            var ruleBuilderResult2 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Sample rule")
                .WithDateBegin(DateTime.Parse("2021-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Sample formula",
                    Value = "0"
                }))
                .Build();

            var newRule2 = ruleBuilderResult2.Rule;
            var ruleAddPriorityOption2 = RuleAddPriorityOption.ByPriorityNumber(4);

            // Act
            var ruleOperationResult1 = await rulesEngine.AddRuleAsync(newRule1, ruleAddPriorityOption1).ConfigureAwait(false);
            var ruleOperationResult2 = await rulesEngine.AddRuleAsync(newRule2, ruleAddPriorityOption2).ConfigureAwait(false);

            // Assert
            ruleOperationResult1.Should().NotBeNull();
            ruleOperationResult1.IsSuccess.Should().BeTrue();

            ruleOperationResult2.Should().NotBeNull();
            ruleOperationResult2.IsSuccess.Should().BeTrue();

            var inMemoryRulesStorage = serviceProvider.GetService<IInMemoryRulesStorage<ContentTypes, ConditionTypes>>();
            var rulesDataSource = CreateRulesDataSource(inMemoryRulesStorage);
            var rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
            rules.Should().NotBeNull().And.HaveCount(3);
            rules.Should().ContainEquivalentOf(newRule1);
            newRule1.Priority.Should().Be(1, "rule should to priority 1 if inserted at priority 1");
            newRule2.Priority.Should().Be(3, "rule should have priority 3 if inserted at priority 3, given that last rule after insert was at priority 2.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleWithAgeConditionOnTop_NewRuleIsInsertedAndExistentRulePriorityUpdated(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource<ContentTypes, ConditionTypes>(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(rulesEngine, DataSourceFilePath, typeof(Formula));

            var newRuleResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Body Mass Index up to 18 years formula",
                    Value = "weight / ((height + 1) ^ 2)" // Not real, for the sake of the test.
                }))
                .WithCondition(cnb => cnb
                    .AsValued(ConditionTypes.Age)
                    .OfDataType<int>()
                    .WithComparisonOperator(Operators.LesserThanOrEqual)
                    .SetOperand(18)
                    .Build())
                .Build();

            var newRule = newRuleResult.Rule;
            var ruleAddPriorityOption = new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtTop
            };

            // Act
            var ruleOperationResult = await rulesEngine.AddRuleAsync(newRule, ruleAddPriorityOption).ConfigureAwait(false);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();

            var inMemoryRulesStorage = serviceProvider.GetService<IInMemoryRulesStorage<ContentTypes, ConditionTypes>>();
            var rulesDataSource = CreateRulesDataSource(inMemoryRulesStorage);
            var rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
            rules.Should().NotBeNull().And.HaveCount(2);
            rules.Should().ContainEquivalentOf(newRule, o => o.Excluding(x => x.RootCondition.Properties));
            newRule.Priority.Should().Be(1, "rule should to priority 1 if inserted at top.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BodyMassIndex_NoConditions_ReturnsDefaultFormula(bool enableCompilation)
        {
            // Arrange
            string expectedFormulaDescription = "Body Mass Index default formula";
            string expectedFormulaValue = "weight / (height ^ 2)";
            const ContentTypes expectedContent = ContentTypes.BodyMassIndexFormula;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Condition<ConditionTypes>[0];

            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource<ContentTypes, ConditionTypes>(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(rulesEngine, DataSourceFilePath, typeof(Formula));

            // Act
            var actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            var actualFormula = actual.ContentContainer.GetContentAs<Formula>();
            actualFormula.Description.Should().Be(expectedFormulaDescription);
            actualFormula.Value.Should().Be(expectedFormulaValue);
        }

        private static IRulesDataSource<ContentTypes, ConditionTypes> CreateRulesDataSource(IInMemoryRulesStorage<ContentTypes, ConditionTypes> inMemoryRulesStorage)
        {
            var ruleFactory = new RuleFactory<ContentTypes, ConditionTypes>();
            return new InMemoryProviderRulesDataSource<ContentTypes, ConditionTypes>(inMemoryRulesStorage, ruleFactory);
        }
    }
}