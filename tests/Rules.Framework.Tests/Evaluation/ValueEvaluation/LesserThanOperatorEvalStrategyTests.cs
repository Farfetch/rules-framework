namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using FluentAssertions;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Xunit;

    public class LesserThanOperatorEvalStrategyTests
    {
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
    }
}