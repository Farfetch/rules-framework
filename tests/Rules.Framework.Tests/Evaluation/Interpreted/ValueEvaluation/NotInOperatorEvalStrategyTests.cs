<<<<<<<< HEAD:tests/Rules.Framework.Tests/Evaluation/Classic/ValueEvaluation/NotInOperatorEvalStrategyTests.cs
namespace Rules.Framework.Tests.Evaluation.Classic.ValueEvaluation
{
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
========
namespace Rules.Framework.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation;
>>>>>>>> master:tests/Rules.Framework.Tests/Evaluation/Interpreted/ValueEvaluation/NotInOperatorEvalStrategyTests.cs
    using Xunit;

    public class NotInOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenInteger1AndCollectionContainingInteger1_ReturnsFalse()
        {
            // Arrange
            object leftOperand = 1;
            var rightOperand = Enumerable.Range(1, 3).Cast<object>();

            var notInOperatorEvalStrategy = new NotInOperatorEvalStrategy();

            // Act
            var result = notInOperatorEvalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenInteger2AndCollectionNotContainingInteger2_ReturnsTrue()
        {
            // Arrange
            object leftOperand = 2;
            var rightOperand = Enumerable.Range(6, 3).Cast<object>();

            var notInOperatorEvalStrategy = new NotInOperatorEvalStrategy();

            // Act
            var result = notInOperatorEvalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            result.Should().BeTrue();
        }
    }
}