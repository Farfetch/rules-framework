namespace Rules.Framework.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Extensions;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleExtensionsTests
    {
        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithComposedCondition_Success()
        {
            var expectedRuleContent = "Type1";

            var expectedRootCondition = new ComposedConditionNode<string>(LogicalOperators.And, new List<IConditionNode<string>>
            {
                new ValueConditionNode<string>(DataTypes.Boolean, ConditionType.IsVip.ToString(), Operators.Equal, true),
                new ComposedConditionNode<string>(LogicalOperators.Or, new List<IConditionNode<string>>
                {
                    new ValueConditionNode<string>(DataTypes.String, ConditionType.IsoCurrency.ToString(), Operators.Equal, "EUR"),
                    new ValueConditionNode<string>(DataTypes.String, ConditionType.IsoCurrency.ToString(), Operators.Equal, "USD")
                })
            });

            var rootComposedCondition = new RootConditionNodeBuilder<ConditionType>()
                .And(a => a
                    .Value(ConditionType.IsVip, Operators.Equal, true)
                    .Or(o => o
                        .Value(ConditionType.IsoCurrency, Operators.Equal, "EUR")
                        .Value(ConditionType.IsoCurrency, Operators.Equal, "USD")
                    )
                );

            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentType.Type1, expectedRuleContent)
                .WithCondition(rootComposedCondition)
                .Build();

            var rule = ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition));
            var content = genericRule.ContentContainer.GetContentAs<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<ComposedConditionNode<string>>();

            var genericComposedRootCondition = genericRule.RootCondition as ComposedConditionNode<string>;
            genericComposedRootCondition.Should().BeEquivalentTo(expectedRootCondition, options => options.ComparingByMembers<ComposedConditionNode<string>>());
        }

        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithoutConditions_Success()
        {
            var expectedRuleContent = "Type2";

            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentType.Type2, expectedRuleContent)
                .Build();

            var rule = ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config.Excluding(r => r.ContentContainer));
            var content = genericRule.ContentContainer.GetContentAs<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeNull();
        }

        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithValueCondition_Success()
        {
            var expectedRuleContent = "Type1";
            var expectedRootCondition = new ValueConditionNode<ConditionType>(DataTypes.Integer, ConditionType.NumberOfSales, Operators.GreaterThan, 1000);

            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentType.Type1, expectedRuleContent)
                .WithCondition(expectedRootCondition)
                .Build();

            var rule = ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition));
            var content = genericRule.ContentContainer.GetContentAs<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<ValueConditionNode<string>>();

            var genericValueRootCondition = genericRule.RootCondition as ValueConditionNode<string>;
            genericValueRootCondition.ConditionType.Should().Be(expectedRootCondition.ConditionType.ToString());
            genericValueRootCondition.DataType.Should().Be(expectedRootCondition.DataType);
            genericValueRootCondition.LogicalOperator.Should().Be(expectedRootCondition.LogicalOperator);
            genericValueRootCondition.Operand.Should().Be(expectedRootCondition.Operand);
            genericValueRootCondition.Operator.Should().Be(expectedRootCondition.Operator);
        }
    }
}