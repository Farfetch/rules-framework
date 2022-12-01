namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class ConditionsTreeAnalyzerTests
    {
        [Fact]
        public void AreAllSearchConditionsPresent_GivenComposedConditionNodeWithAllConditionsOnDictionary_ReturnsTrue()
        {
            // Arrange
            var condition1 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.In, new[] { "US", "CA" });
            var condition2 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.NotEqual, "SGD");
            var condition3 = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.Equal, false);

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Or,
                new IConditionNode<ConditionType>[] { condition1, condition2, condition3 });

            var conditions = new Dictionary<ConditionType, object>
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsoCountryCode,
                    "PT"
                }
            };

            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer<ConditionType>();

            // Act
            var result = conditionsTreeAnalyzer.AreAllSearchConditionsPresent(composedConditionNode, conditions);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AreAllSearchConditionsPresent_GivenComposedConditionNodeWithAndOperatorAndUnknownConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            var condition1 = new StubConditionNode<ConditionType>();
            var condition2 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Or,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            var conditions = new Dictionary<ConditionType, object>()
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsoCountryCode,
                    "PT"
                }
            };

            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer<ConditionType>();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => conditionsTreeAnalyzer.AreAllSearchConditionsPresent(composedConditionNode, conditions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported condition node: 'StubConditionNode`1'.");
        }

        [Fact]
        public void AreAllSearchConditionsPresent_GivenComposedConditionNodeWithMissingConditionOnDictionary_ReturnsFalse()
        {
            // Arrange
            var condition1 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.In, new[] { "US", "CA" });
            var condition3 = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.Equal, false);

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Or,
                new IConditionNode<ConditionType>[] { condition1, condition3 });

            var conditions = new Dictionary<ConditionType, object>()
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsoCountryCode,
                    "PT"
                }
            };

            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer<ConditionType>();

            // Act
            var result = conditionsTreeAnalyzer.AreAllSearchConditionsPresent(composedConditionNode, conditions);

            // Assert
            result.Should().BeFalse();
        }
    }
}