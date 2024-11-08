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
                        Condition = ConditionNames.IsVip,
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
                                Condition = ConditionNames.IsoCurrency,
                                DataType = DataTypes.String,
                                LogicalOperator = LogicalOperators.Eval,
                                Operand = "EUR",
                                Operator = Operators.Equal
                            },
                            new
                            {
                                Condition = ConditionNames.IsoCurrency,
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

            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>("Dummy Rule")
                .InRuleset(RulesetNames.Type1)
                .SetContent(expectedRuleContent)
                .Since(DateTime.Parse("2018-01-01"))
                .ApplyWhen(b => b
                    .And(a => a
                        .Value(ConditionNames.IsVip, Operators.Equal, true)
                        .Or(o => o
                            .Value(ConditionNames.IsoCurrency, Operators.Equal, "EUR")
                            .Value(ConditionNames.IsoCurrency, Operators.Equal, "USD")
                        )
                    )
                )
                .Build();

            var rule = (Rule)ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule<RulesetNames, ConditionNames>();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition)
                .Excluding(r => r.Ruleset));
            var content = genericRule.ContentContainer.GetContentAs<object>();
            content.Should().BeOfType<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionNames>>();

            var genericComposedRootCondition = genericRule.RootCondition as ComposedConditionNode<ConditionNames>;
            genericComposedRootCondition.Should().BeEquivalentTo(expectedRootCondition, config => config.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithoutConditions_Success()
        {
            var expectedRuleContent = "Type2";

            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>("Dummy Rule")
                .InRuleset(RulesetNames.Type2)
                .SetContent(expectedRuleContent)
                .Since(DateTime.Parse("2018-01-01"))
                .Build();

            var rule = (Rule)ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule<RulesetNames, ConditionNames>();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.Ruleset));
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
                ConditionType = ConditionNames.NumberOfSales,
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operator = Operators.GreaterThan,
                Operand = 1000
            };

            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>("Dummy Rule")
                .InRuleset(RulesetNames.Type1)
                .SetContent(expectedRuleContent)
                .Since(DateTime.Parse("2018-01-01"))
                .ApplyWhen(ConditionNames.NumberOfSales, Operators.GreaterThan, 1000)
                .Build();

            var rule = (Rule)ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule<RulesetNames, ConditionNames>();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition)
                .Excluding(r => r.Ruleset));
            var content = genericRule.ContentContainer.GetContentAs<object>();
            content.Should().BeOfType<string>();
            content.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<ValueConditionNode<ConditionNames>>();

            var genericValueRootCondition = genericRule.RootCondition as ValueConditionNode<ConditionNames>;
            genericValueRootCondition.Condition.Should().Be(expectedRootCondition.ConditionType);
            genericValueRootCondition.DataType.Should().Be(expectedRootCondition.DataType);
            genericValueRootCondition.LogicalOperator.Should().Be(expectedRootCondition.LogicalOperator);
            genericValueRootCondition.Operand.Should().Be(expectedRootCondition.Operand);
            genericValueRootCondition.Operator.Should().Be(expectedRootCondition.Operator);
        }
    }
}