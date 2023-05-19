namespace Rules.Framework.Tests.Builder
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Serialization;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleBuilderTests
    {
        [Fact]
        public void NewRule_GivenRuleWithComposedCondition_BuildsAndReturnsRule()
        {
            // Arrange
            string ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var contentType = ContentType.Type1;
            string content = "Content";

            // Act
            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(ruleName)
                .WithDateBegin(dateBegin)
                .WithContent(contentType, content)
                .WithCondition(c => c
                    .Or(o => o
                        .Value(ConditionType.IsoCountryCode, Operators.Equal, "PT")
                        .And(a => a
                            .Value(ConditionType.NumberOfSales, Operators.GreaterThan, 1000)
                            .Value(ConditionType.IsoCurrency, Operators.In, new[] { "EUR", "USD" })
                     )))
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
            rule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionType>>();
            var rootCondition = rule.RootCondition as ComposedConditionNode<ConditionType>;
            rootCondition.LogicalOperator.Should().Be(LogicalOperators.Or);
            rootCondition.ChildConditionNodes.Should().HaveCount(2);
            var childNodes = rootCondition.ChildConditionNodes.ToList();

            // first child node
            childNodes[0].Should().BeOfType<ValueConditionNode<ConditionType>>();
            var isoCountryChildNode = childNodes[0] as ValueConditionNode<ConditionType>;
            isoCountryChildNode.ConditionType.Should().Be(ConditionType.IsoCountryCode);
            isoCountryChildNode.Operator.Should().Be(Operators.Equal);
            isoCountryChildNode.Operand.Should().Be("PT");

            // second child nodes
            childNodes[1].Should().BeOfType<ComposedConditionNode<ConditionType>>();
            var composedChildNode = childNodes[1] as ComposedConditionNode<ConditionType>;
            composedChildNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedChildNode.ChildConditionNodes.Should().HaveCount(2);
            var composedChildNodes = composedChildNode.ChildConditionNodes.ToList();

            composedChildNodes[0].Should().BeOfType<ValueConditionNode<ConditionType>>();
            var numberOfSalesChildNode = composedChildNodes[0] as ValueConditionNode<ConditionType>;
            numberOfSalesChildNode.ConditionType.Should().Be(ConditionType.NumberOfSales);
            numberOfSalesChildNode.Operator.Should().Be(Operators.GreaterThan);
            numberOfSalesChildNode.Operand.Should().Be(1000);

            composedChildNodes[1].Should().BeOfType<ValueConditionNode<ConditionType>>();
            var isoCurrencyChildNode = composedChildNodes[1] as ValueConditionNode<ConditionType>;
            isoCurrencyChildNode.ConditionType.Should().Be(ConditionType.IsoCurrency);
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
            var contentType = ContentType.Type1;
            string content = "Content";
            const ConditionType conditionType = ConditionType.NumberOfSales;
            const int conditionValue = 40;
            var conditionOperator = containsOperator;
            const DataTypes dataType = DataTypes.Integer;

            // Act
            RuleBuilderResult<ContentType, ConditionType> ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(ruleName)
                .WithDateBegin(dateBegin)
                .WithContent(contentType, content)
                .WithCondition(conditionType, conditionOperator, conditionValue)
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
            string ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var contentType = ContentType.Type1;
            string content = "Content";
            const ConditionType conditionType = ConditionType.IsoCountryCode;
            const string conditionValue = "PT";
            var conditionOperator = containsOperator;
            const LogicalOperators logicalOperator = LogicalOperators.Eval;
            const DataTypes dataType = DataTypes.String;

            // Act
            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(ruleName)
                .WithDateBegin(dateBegin)
                .WithContent(contentType, content)
                .WithCondition(c => c.Value(conditionType, conditionOperator, conditionValue))
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

            var rootCondition = rule.RootCondition as IValueConditionNode<ConditionType>;
            rootCondition.ConditionType.Should().Be(conditionType);
            rootCondition.DataType.Should().Be(dataType);
            rootCondition.LogicalOperator.Should().Be(logicalOperator);
            rootCondition.Operator.Should().Be(conditionOperator);
        }

        // TODO create test for WithCondition() with composed condition

        [Fact]
        public void NewRule_WithSerializedContent_GivenNullContentSerializationProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var ruleBuilder = RuleBuilder.NewRule<ContentType, ConditionType>();
            IContentSerializationProvider<ContentType> contentSerializationProvider = null;

            // Act
            var argumentNullException = Assert
                .Throws<ArgumentNullException>(() => ruleBuilder.WithSerializedContent(ContentType.Type1, "TEST", contentSerializationProvider));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(contentSerializationProvider));
        }

        [Fact]
        public void NewRule_WithSerializedContent_SetsContentAsSerializedContent()
        {
            // Arrange
            string ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var contentType = ContentType.Type1;
            string content = "TEST";

            var contentSerializer = Mock.Of<IContentSerializer>();
            Mock.Get(contentSerializer)
                .Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(content);

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();
            Mock.Get(contentSerializationProvider)
                .Setup(x => x.GetContentSerializer(contentType))
                .Returns(contentSerializer);

            // Act
            var ruleBuilderResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(ruleName)
                .WithDateBegin(dateBegin)
                .WithSerializedContent(contentType, content, contentSerializationProvider)
                .Build();

            // Assert
            var ruleContent = ruleBuilderResult.Rule.ContentContainer;
            ruleContent.Should().NotBeNull();
            ruleContent.Should().NotBeNull().And.BeOfType<SerializedContentContainer<ContentType>>();
            ruleContent.ContentType.Should().Be(contentType);
            ruleContent.GetContentAs<string>().Should().Be(content);
        }
    }
}