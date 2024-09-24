namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class ConditionsTreeAnalyzerTests
    {
        [Fact]
        public void AreAllSearchConditionsPresent_GivenComposedConditionNodeWithAllConditionsOnDictionary_ReturnsTrue()
        {
            // Arrange
            var condition1 = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.In, new[] { "US", "CA" });
            var condition2 = new ValueConditionNode(DataTypes.String, ConditionType.IsoCurrency.ToString(), Operators.NotEqual, "SGD");
            var condition3 = new ValueConditionNode(DataTypes.Boolean, ConditionType.IsVip.ToString(), Operators.Equal, false);

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Or,
                new IConditionNode[] { condition1, condition2, condition3 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionType.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionType.IsoCountryCode.ToString(),
                    "PT"
                }
            };

            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer();

            // Act
            var result = conditionsTreeAnalyzer.AreAllSearchConditionsPresent(composedConditionNode, conditions);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AreAllSearchConditionsPresent_GivenComposedConditionNodeWithAndOperatorAndUnknownConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            var condition1 = new StubConditionNode();
            var condition2 = new ValueConditionNode(DataTypes.String, ConditionType.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Or,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionType.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionType.IsoCountryCode.ToString(),
                    "PT"
                }
            };

            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => conditionsTreeAnalyzer.AreAllSearchConditionsPresent(composedConditionNode, conditions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported condition node: 'StubConditionNode'.");
        }

        [Fact]
        public void AreAllSearchConditionsPresent_GivenComposedConditionNodeWithMissingConditionOnDictionary_ReturnsFalse()
        {
            // Arrange
            var condition1 = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.In, new[] { "US", "CA" });
            var condition3 = new ValueConditionNode(DataTypes.Boolean, ConditionType.IsVip.ToString(), Operators.Equal, false);

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Or,
                new IConditionNode[] { condition1, condition3 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionType.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionType.IsoCountryCode.ToString(),
                    "PT"
                }
            };

            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer();

            // Act
            var result = conditionsTreeAnalyzer.AreAllSearchConditionsPresent(composedConditionNode, conditions);

            // Assert
            result.Should().BeFalse();
        }
    }
}