namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class LesserThanOperatorEvalStrategyTests
    {
        public static IEnumerable<object[]> UnsupportedOperandTypesCombinations => new[]
        {
            new[] { 1, new object() },
            new[] { new object(), 1 }
        };

        [Fact]
        public void Eval_GivenAsIntegers0And1_ReturnsTrue()
        {
            // Assert
            int expectedLeftOperand = 0;
            int expectedRightOperand = 1;

            LesserThanOperatorEvalStrategy sut = new LesserThanOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsFalse()
        {
            // Assert
            int expectedLeftOperand = 1;
            int expectedRightOperand = 1;

            LesserThanOperatorEvalStrategy sut = new LesserThanOperatorEvalStrategy();

            // Act
            bool actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenAsIntegers2And1_ReturnsFalse()
        {
            // Assert
            int expectedLeftOperand = 2;
            int expectedRightOperand = 1;

            LesserThanOperatorEvalStrategy sut = new LesserThanOperatorEvalStrategy();

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
            LesserThanOperatorEvalStrategy sut = new LesserThanOperatorEvalStrategy();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}