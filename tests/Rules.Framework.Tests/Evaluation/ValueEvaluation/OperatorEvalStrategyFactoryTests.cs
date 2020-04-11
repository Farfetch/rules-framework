namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Xunit;

    public class OperatorEvalStrategyFactoryTests
    {
        [Theory]
        [InlineData(Operators.Equal, typeof(EqualOperatorEvalStrategy))]
        [InlineData(Operators.NotEqual, typeof(NotEqualOperatorEvalStrategy))]
        [InlineData(Operators.GreaterThan, typeof(GreaterThanOperatorEvalStrategy))]
        [InlineData(Operators.GreaterThanOrEqual, typeof(GreaterThanOrEqualOperatorEvalStrategy))]
        [InlineData(Operators.LesserThan, typeof(LesserThanOperatorEvalStrategy))]
        [InlineData(Operators.LesserThanOrEqual, typeof(LesserThanOrEqualOperatorEvalStrategy))]
        public void GetOperatorEvalStrategy_GivenOperator_ReturnsOperatorEvalStrategy(Operators @operator, Type type)
        {
            // Arrange
            Operators expectedOperator = @operator;
            Type expectedType = type;

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            IOperatorEvalStrategy actual = sut.GetOperatorEvalStrategy(expectedOperator);

            // Assert
            actual.Should().NotBeNull().And.BeOfType(expectedType);
        }

        [Fact]
        public void GetOperatorEvalStrategy_GivenUnknownOperator_ThrowsNotSupportedException()
        {
            // Arrange
            Operators expectedOperator = (Operators)(-1);

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetOperatorEvalStrategy(expectedOperator));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Operator evaluation is not supported for operator '-1'.");
        }
    }
}