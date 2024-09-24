namespace Rules.Framework.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.Generic;
    using Rules.Framework.Generic.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class GenericRuleExtensionsTests
    {
        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithComposedCondition_Success()
        {
            var expectedRuleContent = "Type1";

            var expectedRootCondition = new
            {
                ChildConditionNodes = new List<object>
                {
                    new
                    {
                        ConditionType = ConditionType.IsVip,
                        DataType = DataTypes.Boolean,
                        LogicalOperator = LogicalOperators.Eval,
                        Operand = true,
                        Operator = Operators.Equal
                    },
                    new
                    {
                        ChildConditionNodes = new List<object>
                        {
                            new
                            {
                                ConditionType = ConditionType.IsoCurrency,
                                DataType = DataTypes.String,
                                LogicalOperator = LogicalOperators.Eval,
                                Operand = "EUR",
                                Operator = Operators.Equal
                            },
                            new
                            {
                                ConditionType = ConditionType.IsoCurrency,
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

            var ruleBuilderResult = Rule.New<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentType.Type1, expectedRuleContent)
                .WithCondition(b => b
                    .And(a => a
                        .Value(ConditionType.IsVip, Operators.Equal, true)
                        .Or(o => o
                            .Value(ConditionType.IsoCurrency, Operators.Equal, "EUR")
                            .Value(ConditionType.IsoCurrency, Operators.Equal, "USD")
                        )
                    )
                )
                .Build();

            var rule = (Rule)ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule<ContentType, ConditionType>();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition)
                .Excluding(r => r.ContentType));
            var content = genericRule.ContentContainer.GetContentAs<object>();
            content.Should().BeOfType<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionType>>();

            var genericComposedRootCondition = genericRule.RootCondition as ComposedConditionNode<ConditionType>;
            genericComposedRootCondition.Should().BeEquivalentTo(expectedRootCondition, config => config.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithoutConditions_Success()
        {
            var expectedRuleContent = "Type2";

            var ruleBuilderResult = Rule.New<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentType.Type2, expectedRuleContent)
                .Build();

            var rule = (Rule)ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule<ContentType, ConditionType>();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.ContentType));
            var content = genericRule.ContentContainer.GetContentAs<object>();
            content.Should().BeOfType<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeNull();
        }

        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithValueCondition_Success()
        {
            var expectedRuleContent = "Type1";
            var expectedRootCondition = new
            {
                ConditionType = ConditionType.NumberOfSales,
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operator = Operators.GreaterThan,
                Operand = 1000
            };

            var ruleBuilderResult = Rule.New<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentType.Type1, expectedRuleContent)
                .WithCondition(ConditionType.NumberOfSales, Operators.GreaterThan, 1000)
                .Build();

            var rule = (Rule)ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule<ContentType, ConditionType>();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition)
                .Excluding(r => r.ContentType));
            var content = genericRule.ContentContainer.GetContentAs<object>();
            content.Should().BeOfType<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<ValueConditionNode<ConditionType>>();

            var genericValueRootCondition = genericRule.RootCondition as ValueConditionNode<ConditionType>;
            genericValueRootCondition.ConditionType.Should().Be(expectedRootCondition.ConditionType);
            genericValueRootCondition.DataType.Should().Be(expectedRootCondition.DataType);
            genericValueRootCondition.LogicalOperator.Should().Be(expectedRootCondition.LogicalOperator);
            genericValueRootCondition.Operand.Should().Be(expectedRootCondition.Operand);
            genericValueRootCondition.Operator.Should().Be(expectedRootCondition.Operator);
        }
    }
}