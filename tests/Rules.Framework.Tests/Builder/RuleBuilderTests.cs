namespace Rules.Framework.Tests.Builder
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Builder.Generic.RulesBuilder;
    using Rules.Framework.Generic.ConditionNodes;
    using Rules.Framework.Serialization;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleBuilderTests
    {
        [Fact]
        public void NewRule_GivenRuleWithComposedCondition_BuildsAndReturnsRule()
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "Content";

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .OnRuleset(ruleset)
                .SetContent(content)
                .Since(dateBegin)
                .ApplyWhen(c => c
                    .Or(o => o
                        .Value(ConditionNames.IsoCountryCode, Operators.Equal, "PT")
                        .And(a => a
                            .Value(ConditionNames.NumberOfSales, Operators.GreaterThan, 1000)
                            .Value(ConditionNames.IsoCurrency, Operators.In, new[] { "EUR", "USD" })
                        )
                    )
                )
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();

            var rule = ruleBuilderResult.Rule;
            rule.Name.Should().Be(ruleName);
            rule.DateBegin.Should().Be(dateBegin);
            rule.DateEnd.Should().BeNull();
            rule.ContentContainer.Should().NotBeNull();

            // root node
            rule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionNames>>();
            var rootCondition = rule.RootCondition as ComposedConditionNode<ConditionNames>;
            rootCondition.LogicalOperator.Should().Be(LogicalOperators.Or);
            rootCondition.ChildConditionNodes.Should().HaveCount(2);
            var childNodes = rootCondition.ChildConditionNodes.ToList();

            // first child node
            childNodes[0].Should().BeOfType<ValueConditionNode<ConditionNames>>();
            var isoCountryChildNode = childNodes[0] as ValueConditionNode<ConditionNames>;
            isoCountryChildNode.Condition.Should().Be(ConditionNames.IsoCountryCode);
            isoCountryChildNode.Operator.Should().Be(Operators.Equal);
            isoCountryChildNode.Operand.Should().Be("PT");

            // second child nodes
            childNodes[1].Should().BeOfType<ComposedConditionNode<ConditionNames>>();
            var composedChildNode = childNodes[1] as ComposedConditionNode<ConditionNames>;
            composedChildNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedChildNode.ChildConditionNodes.Should().HaveCount(2);
            var composedChildNodes = composedChildNode.ChildConditionNodes.ToList();

            composedChildNodes[0].Should().BeOfType<ValueConditionNode<ConditionNames>>();
            var numberOfSalesChildNode = composedChildNodes[0] as ValueConditionNode<ConditionNames>;
            numberOfSalesChildNode.Condition.Should().Be(ConditionNames.NumberOfSales);
            numberOfSalesChildNode.Operator.Should().Be(Operators.GreaterThan);
            numberOfSalesChildNode.Operand.Should().Be(1000);

            composedChildNodes[1].Should().BeOfType<ValueConditionNode<ConditionNames>>();
            var isoCurrencyChildNode = composedChildNodes[1] as ValueConditionNode<ConditionNames>;
            isoCurrencyChildNode.Condition.Should().Be(ConditionNames.IsoCurrency);
            isoCurrencyChildNode.Operator.Should().Be(Operators.In);
            isoCurrencyChildNode.Operand.Should().BeEquivalentTo(new[] { "EUR", "USD" });
        }

        [Theory]
        [InlineData(Operators.Contains)]
        [InlineData(Operators.NotContains)]
        public void NewRule_GivenRuleWithIntegerConditionTypeAndContainsOperator_ReturnsInvalidRuleResult(Operators containsOperator)
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "Content";
            const ConditionNames conditionType = ConditionNames.NumberOfSales;
            const int conditionValue = 40;
            var conditionOperator = containsOperator;
            const DataTypes dataType = DataTypes.Integer;

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .OnRuleset(ruleset)
                .SetContent(content)
                .Since(dateBegin)
                .ApplyWhen(conditionType, conditionOperator, conditionValue)
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeFalse();
            ruleBuilderResult.Rule.Should().BeNull();

            ruleBuilderResult.Errors.Should().NotBeNull().And.NotBeEmpty();
            ruleBuilderResult.Errors.Should().ContainMatch($"*{dataType}*{conditionOperator}*");
        }

        [Theory]
        [InlineData(Operators.Contains)]
        [InlineData(Operators.NotContains)]
        public void NewRule_GivenRuleWithStringConditionTypeAndContainsOperator_BuildsAndReturnsRule(Operators containsOperator)
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "Content";
            const ConditionNames conditionType = ConditionNames.IsoCountryCode;
            const string conditionValue = "PT";
            var conditionOperator = containsOperator;
            const LogicalOperators logicalOperator = LogicalOperators.Eval;
            const DataTypes dataType = DataTypes.String;

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .OnRuleset(ruleset)
                .SetContent(content)
                .Since(dateBegin)
                .ApplyWhen(c => c.Value(conditionType, conditionOperator, conditionValue))
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();
            ruleBuilderResult.Rule.Should().NotBeNull();

            var rule = ruleBuilderResult.Rule;

            rule.Name.Should().Be(ruleName);
            rule.DateBegin.Should().Be(dateBegin);
            rule.DateEnd.Should().BeNull();
            rule.ContentContainer.Should().NotBeNull();
            rule.RootCondition.Should().NotBeNull();
            rule.RootCondition.Should().BeAssignableTo<IValueConditionNode<ConditionNames>>();

            var rootCondition = rule.RootCondition as IValueConditionNode<ConditionNames>;
            rootCondition.Condition.Should().Be(conditionType);
            rootCondition.DataType.Should().Be(dataType);
            rootCondition.LogicalOperator.Should().Be(logicalOperator);
            rootCondition.Operator.Should().Be(conditionOperator);
        }

        // TODO create test for WithCondition() with composed condition

        [Fact]
        public void NewRule_WithSerializedContent_GivenNullContentSerializationProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var ruleBuilder = new RuleBuilder<RulesetNames, ConditionNames>("My rule used for testing purposes");
            IContentSerializationProvider contentSerializationProvider = null;

            // Act
            var argumentNullException = Assert
                .Throws<ArgumentNullException>(() => ruleBuilder.SetContent(new object(), contentSerializationProvider));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(contentSerializationProvider));
        }

        [Fact]
        public void NewRule_WithSerializedContent_SetsContentAsSerializedContent()
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "TEST";

            var contentSerializer = Mock.Of<IContentSerializer>();
            Mock.Get(contentSerializer)
                .Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(content);

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider>();
            Mock.Get(contentSerializationProvider)
                .Setup(x => x.GetContentSerializer(ruleset.ToString()))
                .Returns(contentSerializer);

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .OnRuleset(ruleset)
                .SetContent(content, contentSerializationProvider)
                .Since(dateBegin)
                .Build();

            // Assert
            ruleBuilderResult.Rule.Ruleset.Should().Be(ruleset);
            var ruleContent = ruleBuilderResult.Rule.ContentContainer;
            ruleContent.Should().NotBeNull().And.BeOfType<SerializedContentContainer>();
            ruleContent.GetContentAs<string>().Should().Be(content);
        }
    }
}