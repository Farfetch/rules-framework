namespace Rules.Framework.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Extensions;
    using Rules.Framework.Generic;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class GenericRuleExtensionsTests
    {
        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithComposedConditions_Success()
        {
            var expectedRuleContent = "Type1";

            // TODO maybe better to create a builder for building GenericConditions (and GenericRules)
            var expectedRootCondition = new GenericComposedConditionNode<GenericConditionType>
            {
                ChildConditionNodes = new List<GenericConditionNode<GenericConditionType>>()
                {
                    new GenericValueConditionNode<GenericConditionType>
                    {
                        ConditionTypeName = ConditionType.IsVip.ToString(),
                        DataType = DataTypes.Boolean,
                        Operand = true,
                        Operator = Operators.Equal
                    },
                    new GenericComposedConditionNode<GenericConditionType>
                    {
                        ChildConditionNodes = new List<GenericConditionNode<GenericConditionType>>()
                        {
                            new GenericValueConditionNode<GenericConditionType>
                            {
                                ConditionTypeName = ConditionType.IsoCurrency.ToString(),
                                DataType = DataTypes.String,
                                Operand = "EUR",
                                Operator = Operators.Equal
                            },
                            new GenericValueConditionNode<GenericConditionType>
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
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type1, (t) => expectedRuleContent))
                .WithCondition(composedCondition)
                .Build();

            var rule = ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition));
            genericRule.ContentContainer.Should().BeOfType<string>();
            genericRule.ContentContainer.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<GenericComposedConditionNode<GenericConditionType>>();

            var genericComposedRootCondition = genericRule.RootCondition as GenericComposedConditionNode<GenericConditionType>;
            genericComposedRootCondition.Should().BeEquivalentTo(expectedRootCondition, config => config.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void GenericRuleExtensions_ToGenericRule_WithoutConditions_Success()
        {
            var expectedRuleContent = "Type2";

            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Dummy Rule")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type2, (t) => expectedRuleContent))
                .Build();

            var rule = ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config.Excluding(r => r.ContentContainer));
            genericRule.ContentContainer.Should().BeOfType<string>();
            genericRule.ContentContainer.Should().Be(expectedRuleContent);
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
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type1, (t) => expectedRuleContent))
                .WithCondition(expectedRootCondition)
                .Build();

            var rule = ruleBuilderResult.Rule;

            // Act
            var genericRule = rule.ToGenericRule();

            // Assert
            genericRule.Should().BeEquivalentTo(rule, config => config
                .Excluding(r => r.ContentContainer)
                .Excluding(r => r.RootCondition));
            genericRule.ContentContainer.Should().BeOfType<string>();
            genericRule.ContentContainer.Should().Be(expectedRuleContent);
            genericRule.RootCondition.Should().BeOfType<GenericValueConditionNode<GenericConditionType>>();

            var genericValueRootCondition = genericRule.RootCondition as GenericValueConditionNode<GenericConditionType>;
            genericValueRootCondition.Should().BeEquivalentTo(expectedRootCondition, config => config
                .Excluding(r => r.ConditionType)
                .Excluding(r => r.LogicalOperator));
            genericValueRootCondition.ConditionTypeName.Should().Be(expectedRootCondition.ConditionType.ToString());
        }
    }
}