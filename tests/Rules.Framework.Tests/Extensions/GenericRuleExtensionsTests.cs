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
                                Operand = "EUR",
                                Operator = Operators.Equal
                            },
                            new GenericValueConditionNode
                            {
                                ConditionTypeName = ConditionType.IsoCurrency.ToString(),
                                DataType = DataTypes.String,
                                Operand = "USD",
                                Operator = Operators.Equal
                            }
                        },
                        LogicalOperator = LogicalOperators.Or
                    }
                },
                LogicalOperator = LogicalOperators.And
            };

            var composedCondition = new ConditionNodeBuilder<ConditionType>()
                .AsComposed()
                      .WithLogicalOperator(LogicalOperators.And)
                           .AddCondition(z => z
                               .AsValued(ConditionType.IsVip).OfDataType<bool>()
                               .WithComparisonOperator(Operators.Equal)
                               .SetOperand(true)
                               .Build()
                           )
                           .AddCondition(sub => sub.AsComposed()
                                .WithLogicalOperator(LogicalOperators.Or)
                                    .AddCondition(tri => tri
                                        .AsValued(ConditionType.IsoCurrency).OfDataType<string>()
                                        .WithComparisonOperator(Operators.Equal)
                                        .SetOperand("EUR").Build())
                                    .AddCondition(tri => tri
                                        .AsValued(ConditionType.IsoCurrency).OfDataType<string>()
                                        .WithComparisonOperator(Operators.Equal)
                                        .SetOperand("USD").Build())
                                .Build())
                      .Build();

            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type1, (_) => expectedRuleContent))
                .WithCondition(composedCondition)
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
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type2, (_) => expectedRuleContent))
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
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type1, (_) => expectedRuleContent))
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
            genericValueRootCondition.Should().BeEquivalentTo(expectedRootCondition, config => config
                .Excluding(r => r.ConditionType)
                .Excluding(r => r.LogicalOperator));
            genericValueRootCondition.ConditionTypeName.Should().Be(expectedRootCondition.ConditionType.ToString());
        }
    }
}