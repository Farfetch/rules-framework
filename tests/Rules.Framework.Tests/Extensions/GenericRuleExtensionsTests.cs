namespace Rules.Framework.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Extensions;
    using Rules.Framework.Generics;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class GenericRuleExtensionsTests
    {
        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithComposedCondition_Success()
        {
            var expectedRuleContent = "Type1";

            var expectedRootCondition = new GenericComposedConditionNode
            {
                ChildConditionNodes = new List<GenericConditionNode>
                {
                    new GenericValueConditionNode
                    {
                        ConditionTypeName = ConditionType.IsVip.ToString(),
                        DataType = DataTypes.Boolean,
                        LogicalOperator = LogicalOperators.Eval,
                        Operand = true,
                        Operator = Operators.Equal
                    },
                    new GenericComposedConditionNode
                    {
                        ChildConditionNodes = new List<GenericConditionNode>
                        {
                            new GenericValueConditionNode
                            {
                                ConditionTypeName = ConditionType.IsoCurrency.ToString(),
                                DataType = DataTypes.String,
                                LogicalOperator = LogicalOperators.Eval,
                                Operand = "EUR",
                                Operator = Operators.Equal
                            },
                            new GenericValueConditionNode
                            {
                                ConditionTypeName = ConditionType.IsoCurrency.ToString(),
                                DataType = DataTypes.String,
                                LogicalOperator = LogicalOperators.Eval,
                                Operand = "USD",
                                Operator = Operators.Equal
                            }
                        },
                        LogicalOperator = LogicalOperators.Or
                    }
                },
                LogicalOperator = LogicalOperators.And
            };

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
            genericRule.Content.Should().BeOfType<string>();
            genericRule.Content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<GenericComposedConditionNode>();

            var genericComposedRootCondition = genericRule.RootCondition as GenericComposedConditionNode;
            genericComposedRootCondition.Should().BeEquivalentTo(expectedRootCondition, config => config.IncludingAllRuntimeProperties());
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
            genericRule.Content.Should().BeOfType<string>();
            genericRule.Content.Should().Be(expectedRuleContent);
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
            genericRule.Content.Should().BeOfType<string>();
            genericRule.Content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<GenericValueConditionNode>();

            var genericValueRootCondition = genericRule.RootCondition as GenericValueConditionNode;
            genericValueRootCondition.ConditionTypeName.Should().Be(expectedRootCondition.ConditionType.ToString());
            genericValueRootCondition.DataType.Should().Be(expectedRootCondition.DataType);
            genericValueRootCondition.LogicalOperator.Should().Be(expectedRootCondition.LogicalOperator);
            genericValueRootCondition.Operand.Should().Be(expectedRootCondition.Operand);
            genericValueRootCondition.Operator.Should().Be(expectedRootCondition.Operator);
        }
    }
}