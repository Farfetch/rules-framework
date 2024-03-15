namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class NotStartsWithOperatorEvalStrategyTests
    {
        private readonly NotStartsWithOperatorEvalStrategy evalStrategy;

        public NotStartsWithOperatorEvalStrategyTests()
        {
            evalStrategy = new NotStartsWithOperatorEvalStrategy();
        }

        [Theory]
        [InlineData(2, "test")]
        [InlineData("test", false)]
        [InlineData(2, false)]
        public void Eval_GivenInvalidOperandCombination_ThrowsNotSupportedOperation(object leftOperand, object rightOperand)
        {
            // Act
            var exception = Assert.Throws<NotSupportedException>(() => evalStrategy.Eval(leftOperand, rightOperand));

            // Assert
            Assert.Contains(nameof(String), exception.Message);
        }

        [Theory]
        [InlineData("Sample text", "random")]
        [InlineData("Sample text", "sample")]
        public void Eval_GivenLeftOperandNotStartingWithRightOperand_ReturnsTrue(string leftOperand, string rightOperand)
        {
            // Act
            var evaluation = evalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            Assert.True(evaluation);
        }

        [Fact]
        public void Eval_GivenLeftOperandStartingWithRightOperand_ReturnsFalse()
        {
            // Arrange
            var leftOperand = "Sample text";
            var rightOperand = "Sample";

            // Act
            var evaluation = evalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            Assert.False(evaluation);
        }
    }
}