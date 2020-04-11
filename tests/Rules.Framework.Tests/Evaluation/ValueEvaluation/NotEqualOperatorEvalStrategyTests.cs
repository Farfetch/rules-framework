namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using FluentAssertions;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Xunit;

    public class NotEqualOperatorEvalStrategyTests
    {
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
    }
}