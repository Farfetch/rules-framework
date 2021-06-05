namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Xunit;

    public class InOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenInteger1AndCollectionContainingInteger1_ReturnsTrue()
        {
            // Arrange
            object leftOperand = 1;
            IEnumerable<object> rightOperand = Enumerable.Range(1, 3).Cast<object>();

            InOperatorEvalStrategy inOperatorEvalStrategy = new InOperatorEvalStrategy();

            // Act
            bool result = inOperatorEvalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenInteger2AndCollectionNotContainingInteger2_ReturnsFalse()
        {
            // Arrange
            object leftOperand = 2;
            IEnumerable<object> rightOperand = Enumerable.Range(6, 3).Cast<object>();

            InOperatorEvalStrategy inOperatorEvalStrategy = new InOperatorEvalStrategy();

            // Act
            bool result = inOperatorEvalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            result.Should().BeFalse();
        }
    }
}