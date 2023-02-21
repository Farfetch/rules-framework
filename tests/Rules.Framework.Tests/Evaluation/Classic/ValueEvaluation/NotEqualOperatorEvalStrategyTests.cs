namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
    using Xunit;

    public class NotEqualOperatorEvalStrategyTests
    {
        public static IEnumerable<object[]> UnsupportedOperandTypesCombinations => new[]
        {
            new[] { 1, new object() },
            new[] { new object(), 1 }
        };

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsFalse()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            NotEqualOperatorEvalStrategy sut = new NotEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And2_ReturnsTrue()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            NotEqualOperatorEvalStrategy sut = new NotEqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [MemberData(nameof(UnsupportedOperandTypesCombinations))]
        [Theory]
        public void Eval_GivenUnsupportedOperandTypes_ThrowsNotSupportedException(object leftOperand, object rightOperand)
        {
            // Arrange
            NotEqualOperatorEvalStrategy sut = new NotEqualOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}