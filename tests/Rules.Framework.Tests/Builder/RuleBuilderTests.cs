namespace Rules.Framework.Tests.Builder
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class RuleBuilderTests
    {
        [Fact]
        public void NewRule_GivenRuleWithIntegerConditionTypeAndContainsOperator_ReturnsInvalidRuleResult()
        {
            // Arrange
            string ruleName = "Rule 1";
            DateTime dateBegin = DateTime.Parse("2021-01-01");
            ContentType contentType = ContentType.Type1;
            string content = "Content";
            const ConditionType conditionType = ConditionType.NumberOfSales;
            const int conditionValue = 40;
            const Operators conditionOperator = Operators.Contains;
            const DataTypes dataType = DataTypes.Integer;

            // Act
            RuleBuilderResult<ContentType, ConditionType> ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(ruleName)
                .WithDateBegin(dateBegin)
                .WithContentContainer(new ContentContainer<ContentType>(contentType, t => content))
                .WithCondition(b =>
                {
                    return b.AsValued(conditionType)
                        .OfDataType<int>()
                        .WithComparisonOperator(conditionOperator)
                        .SetOperand(conditionValue)
                        .Build();
                })
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeFalse();
            ruleBuilderResult.Rule.Should().BeNull();

            ruleBuilderResult.Errors.Should().NotBeNull().And.NotBeEmpty();
            ruleBuilderResult.Errors.Should().ContainMatch($"*{dataType}*{conditionOperator}*");
        }

        [Fact]
        public void NewRule_GivenRuleWithStringConditionTypeAndContainsOperator_BuildsAndReturnsRule()
        {
            // Arrange
            string ruleName = "Rule 1";
            DateTime dateBegin = DateTime.Parse("2021-01-01");
            ContentType contentType = ContentType.Type1;
            string content = "Content";
            const ConditionType conditionType = ConditionType.IsoCountryCode;
            const string conditionValue = "PT";
            const Operators conditionOperator = Operators.Contains;
            const LogicalOperators logicalOperator = LogicalOperators.Eval;
            const DataTypes dataType = DataTypes.String;

            // Act
            RuleBuilderResult<ContentType, ConditionType> ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(ruleName)
                .WithDateBegin(dateBegin)
                .WithContentContainer(new ContentContainer<ContentType>(contentType, t => content))
                .WithCondition(b =>
                {
                    return b.AsValued(conditionType)
                        .OfDataType<string>()
                        .WithComparisonOperator(conditionOperator)
                        .SetOperand(conditionValue)
                        .Build();
                })
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();
            ruleBuilderResult.Rule.Should().NotBeNull();

            Rule<ContentType, ConditionType> rule = ruleBuilderResult.Rule;

            rule.Name.Should().Be(ruleName);
            rule.DateBegin.Should().Be(dateBegin);
            rule.DateEnd.Should().BeNull();
            rule.ContentContainer.Should().NotBeNull();
            rule.RootCondition.Should().NotBeNull();
            rule.RootCondition.Should().BeAssignableTo<IValueConditionNode<ConditionType>>();

            IValueConditionNode<ConditionType> rootCondition = rule.RootCondition as IValueConditionNode<ConditionType>;
            rootCondition.ConditionType.Should().Be(conditionType);
            rootCondition.DataType.Should().Be(dataType);
            rootCondition.LogicalOperator.Should().Be(logicalOperator);
            rootCondition.Operator.Should().Be(conditionOperator);
        }
    }
}