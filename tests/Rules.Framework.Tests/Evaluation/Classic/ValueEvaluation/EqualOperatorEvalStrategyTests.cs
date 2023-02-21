namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
    using Xunit;

    public class EqualOperatorEvalStrategyTests
    {
        public static IEnumerable<object[]> UnsupportedOperandTypesCombinations => new[]
        {
            new[] { 1, new object() },
            new[] { new object(), 1 }
        };

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsTrue()

        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            EqualOperatorEvalStrategy sut = new EqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And2_ReturnsFalse()
        {
            // Arrange
            int expectedLeftOperand = 1;
            int expectedRightOperand = 2;

            EqualOperatorEvalStrategy sut = new EqualOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [MemberData(nameof(UnsupportedOperandTypesCombinations))]
        [Theory]
        public void Eval_GivenUnsupportedOperandTypes_ThrowsNotSupportedException(object leftOperand, object rightOperand)
        {
            // Arrange
            EqualOperatorEvalStrategy sut = new EqualOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}