namespace Rules.Framework.IntegrationTests.Tests.Scenario1
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Xunit;

    public class BodyMassIndexTests
    {
        [Fact]
        public async Task BodyMassIndex_NoConditions_ReturnsDefaultFormula()
        {
            // Arrange
            string expectedFormulaDescription = "Body Mass Index default formula";
            string expectedFormulaValue = "weight / (height ^ 2)";
            const ContentTypes expectedContent = ContentTypes.BodyMassIndexFormula;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[0];

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>($@"{Environment.CurrentDirectory}/Tests/Scenario1/BodyMassIndexTests.datasource.json");

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            Formula actualFormula = actual.ContentContainer.GetContentAs<Formula>();
            actualFormula.Description.Should().Be(expectedFormulaDescription);
            actualFormula.Value.Should().Be(expectedFormulaValue);
        }

        [Fact]
        public async Task AddRule_AddingNewRuleWithAgeConditionOnTop_NewRuleIsInsertedAndExistentRulePriorityUpdated()
        {
            // Arrange
            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>($@"{Environment.CurrentDirectory}/Tests/Scenario1/BodyMassIndexTests.datasource.json");

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Build();

            RuleBuilderResult<ContentTypes, ConditionTypes> newRuleResult = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithPriority(60) // Won't matter, we are actually testing that it will assume 1.
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

            Rule<ContentTypes, ConditionTypes> newRule = newRuleResult.Rule;
            RuleAddPriorityOption ruleAddPriorityOption = new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtTop
            };

            // Act
            RuleOperationResult ruleOperationResult = await rulesEngine.AddRuleAsync(newRule, ruleAddPriorityOption).ConfigureAwait(false);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();

            IEnumerable<Rule<ContentTypes, ConditionTypes>> rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs<ContentTypes>()).ConfigureAwait(false);
            rules.Should().NotBeNull().And.HaveCount(2);
            rules.Should().ContainEquivalentOf(newRule);
            newRule.Priority.Should().Be(1, "rule should to priority 1 if inserted at top.");
        }
    }
}