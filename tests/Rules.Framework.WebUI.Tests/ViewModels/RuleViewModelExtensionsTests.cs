namespace Rules.Framework.WebUI.Tests.ViewModels
{
    using System;
    using FluentAssertions;
    using Rules.Framework.WebUI.ViewModels;
    using Xunit;

    public class RuleViewModelExtensionsTests
    {
        [Fact]
        public void ToExportRulesModel_GivenRuleViewModel_ReturnsExportRulesModel()
        {
            // Arrange
            var ruleViewModel = new RuleViewModel
            {
                Active = true,
                Content = new object(),
                DateBegin = DateTime.Parse("2024-10-01Z"),
                DateEnd = DateTime.Parse("2024-10-31Z"),
                Name = "Sample name",
                Priority = 1,
                RootCondition = new ComposedConditionNodeViewModel
                {
                    ChildConditionNodes = new[]
                    {
                        new ValueConditionNodeViewModel
                        {
                            Condition = "Condition1",
                            DataType = "String",
                            LogicalOperator = "Eval",
                            Operand = "xyz",
                            Operator = "Equal",
                        },
                        new ValueConditionNodeViewModel
                        {
                            Condition = "Condition2",
                            DataType = "Integer",
                            LogicalOperator = "Eval",
                            Operand = "123",
                            Operator = "Equal",
                        },
                    },
                    LogicalOperator = "Or",
                },
                Ruleset = "Ruleset1",
            };

            // Act
            var actual = ruleViewModel.ToExportRulesModel();

            // Assert
            actual.Should().NotBeNull();
            actual.Active.Should().BeTrue();
            actual.Content.Should().NotBeNull()
                .And.BeSameAs(ruleViewModel.Content);
            actual.DateBegin.Should().Be(ruleViewModel.DateBegin);
            actual.DateEnd.Should().Be(ruleViewModel.DateEnd);
            actual.Name.Should().Be(ruleViewModel.Name);
            actual.Priority.Should().Be(ruleViewModel.Priority);
            actual.RootCondition.Should().BeEquivalentTo(ruleViewModel.RootCondition);
            actual.Ruleset.Should().Be(ruleViewModel.Ruleset);
        }

        [Fact]
        public void ToViewModel_GivenRule_ReturnsRuleViewModel()
        {
            // Arrange
            var rule = Rule.Create("Sample name")
                .InRuleset("Ruleset1")
                .SetContent(new object())
                .Since(DateTime.Parse("2024-10-01Z"))
                .Until(DateTime.Parse("2024-10-31Z"))
                .ApplyWhen(b => b
                    .Or(or => or
                        .Value("Condition1", Operators.Equal, "xyz")
                        .Value("Condition2", Operators.Equal, 123)
                    )
                )
                .Build()
                .Rule;
            rule.Priority = 1;
            var expected = new RuleViewModel
            {
                Active = true,
                Content = new object(),
                DateBegin = DateTime.Parse("2024-10-01Z"),
                DateEnd = DateTime.Parse("2024-10-31Z"),
                Id = Guid.Parse("7247859b-3519-f813-cd7b-2c23723673ae"),
                Name = "Sample name",
                Priority = 1,
                RootCondition = new ComposedConditionNodeViewModel
                {
                    ChildConditionNodes = new[]
                    {
                        new ValueConditionNodeViewModel
                        {
                            Condition = "Condition1",
                            DataType = "String",
                            LogicalOperator = "Eval",
                            Operand = "xyz",
                            Operator = "Equal",
                        },
                        new ValueConditionNodeViewModel
                        {
                            Condition = "Condition2",
                            DataType = "Integer",
                            LogicalOperator = "Eval",
                            Operand = "123",
                            Operator = "Equal",
                        },
                    },
                    LogicalOperator = "Or",
                },
                Ruleset = "Ruleset1",
            };

            // Act
            var actual = rule.ToViewModel();

            // Assert
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}