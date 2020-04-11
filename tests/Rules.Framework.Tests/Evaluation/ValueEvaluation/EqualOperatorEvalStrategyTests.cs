namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using FluentAssertions;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Xunit;

    public class EqualOperatorEvalStrategyTests
    {
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
    }
}