namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System;
<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/NotEndsWithOperatorEvalStrategyTests.cs
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
========
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/NotEndsWithOperatorEvalStrategyTests.cs
    using Xunit;

    public class NotEndsWithOperatorEvalStrategyTests
    {
        private readonly NotEndsWithOperatorEvalStrategy evalStrategy;

        public NotEndsWithOperatorEvalStrategyTests()
        {
            evalStrategy = new NotEndsWithOperatorEvalStrategy();
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
        [InlineData("Sample text", "Text")]
        public void Eval_GivenLeftOperandNotEndingWithRightOperand_ReturnsTrue(string leftOperand, string rightOperand)
        {
            // Act
            var evaluation = evalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            Assert.True(evaluation);
        }

        [Fact]
        public void Eval_GivenLeftOperandEndingWithRightOperand_ReturnsFalse()
        {
            // Arrange
            var leftOperand = "Sample text";
            var rightOperand = "text";

            // Act
            var evaluation = evalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            Assert.False(evaluation);
        }
    }
}