namespace Rules.Framework.IntegrationTests.Tests.Scenario1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario1;
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
            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource =
                new InMemoryRulesDataSource<ContentTypes, ConditionTypes>(Enumerable.Empty<Rule<ContentTypes, ConditionTypes>>());

            // Arrange
            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            RuleBuilderResult<ContentTypes, ConditionTypes> newRuleResult1 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
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

            Rule<ContentTypes, ConditionTypes> newRule1 = newRuleResult1.Rule;
            RuleAddPriorityOption ruleAddPriorityOption1 = RuleAddPriorityOption.ByPriorityNumber(1);

            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult2 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Sample rule")
                .WithDateBegin(DateTime.Parse("2021-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Sample formula",
                    Value = "0"
                }))
                .Build();

            Rule<ContentTypes, ConditionTypes> newRule2 = ruleBuilderResult2.Rule;
            RuleAddPriorityOption ruleAddPriorityOption2 = RuleAddPriorityOption.ByPriorityNumber(4);

            // Act
            RuleOperationResult ruleOperationResult1 = await rulesEngine.AddRuleAsync(newRule1, ruleAddPriorityOption1).ConfigureAwait(false);
            RuleOperationResult ruleOperationResult2 = await rulesEngine.AddRuleAsync(newRule2, ruleAddPriorityOption2).ConfigureAwait(false);

            // Assert
            ruleOperationResult1.Should().NotBeNull();
            ruleOperationResult1.IsSuccess.Should().BeTrue();

            ruleOperationResult2.Should().NotBeNull();
            ruleOperationResult2.IsSuccess.Should().BeTrue();

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
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
            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            RuleBuilderResult<ContentTypes, ConditionTypes> newRuleResult1 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
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

            Rule<ContentTypes, ConditionTypes> expectedRule1 = newRuleResult1.Rule;
            RuleAddPriorityOption ruleAddPriorityOption1 = RuleAddPriorityOption.ByPriorityNumber(1);

            RuleBuilderResult<ContentTypes, ConditionTypes> ruleBuilderResult2 = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Sample rule")
                .WithDateBegin(DateTime.Parse("2021-01-01"))
                .WithContentContainer(new ContentContainer<ContentTypes>(ContentTypes.BodyMassIndexFormula, (t) => new Formula
                {
                    Description = "Sample formula",
                    Value = "0"
                }))
                .Build();

            Rule<ContentTypes, ConditionTypes> expectedRule2 = ruleBuilderResult2.Rule;
            RuleAddPriorityOption ruleAddPriorityOption2 = RuleAddPriorityOption.ByPriorityNumber(4);

            // Act
            RuleOperationResult ruleOperationResult1 = await rulesEngine.AddRuleAsync(expectedRule1, ruleAddPriorityOption1).ConfigureAwait(false);
            RuleOperationResult ruleOperationResult2 = await rulesEngine.AddRuleAsync(expectedRule2, ruleAddPriorityOption2).ConfigureAwait(false);

            // Assert
            ruleOperationResult1.Should().NotBeNull();
            ruleOperationResult1.IsSuccess.Should().BeTrue();

            ruleOperationResult2.Should().NotBeNull();
            ruleOperationResult2.IsSuccess.Should().BeTrue();

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
            rules.Should().NotBeNull().And.HaveCount(3);
            rules.Should().ContainEquivalentOf(expectedRule1);
            expectedRule1.Priority.Should().Be(1, "rule should to priority 1 if inserted at priority 1");
            expectedRule2.Priority.Should().Be(3, "rule should have priority 3 if inserted at priority 3, given that last rule after insert was at priority 2.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleWithAgeConditionOnTop_NewRuleIsInsertedAndExistentRulePriorityUpdated(bool enableCompilation)
        {
            // Arrange
            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            RuleBuilderResult<ContentTypes, ConditionTypes> newRuleResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
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

            Rule<ContentTypes, ConditionTypes> expectedRule = newRuleResult.Rule;
            RuleAddPriorityOption ruleAddPriorityOption = new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtTop
            };

            // Act
            RuleOperationResult ruleOperationResult = await rulesEngine.AddRuleAsync(expectedRule, ruleAddPriorityOption).ConfigureAwait(false);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
            rules.Should().NotBeNull().And.HaveCount(2);
            rules.Should().ContainEquivalentOf(expectedRule, o => o.Excluding(x => x.RootCondition.Properties));
            expectedRule.Priority.Should().Be(1, "rule should to priority 1 if inserted at top.");
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
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[0];

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>(DataSourceFilePath);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            Formula actualFormula = actual.ContentContainer.GetContentAs<Formula>();
            actualFormula.Description.Should().Be(expectedFormulaDescription);
            actualFormula.Value.Should().Be(expectedFormulaValue);
        }
    }
}