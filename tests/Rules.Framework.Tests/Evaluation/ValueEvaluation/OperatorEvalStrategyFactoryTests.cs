namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.ValueEvaluation;

    [TestClass]
    public class OperatorEvalStrategyFactoryTests
    {
        [TestMethod]
        [DataRow(Operators.Equal, typeof(EqualOperatorEvalStrategy), DisplayName = "EqualOperator")]
        [DataRow(Operators.NotEqual, typeof(NotEqualOperatorEvalStrategy), DisplayName = "NotEqualOperator")]
        [DataRow(Operators.GreaterThan, typeof(GreaterThanOperatorEvalStrategy), DisplayName = "GreaterThanOperator")]
        [DataRow(Operators.GreaterThanOrEqual, typeof(GreaterThanOrEqualOperatorEvalStrategy), DisplayName = "GreaterThanOrEqualOperator")]
        [DataRow(Operators.LesserThan, typeof(LesserThanOperatorEvalStrategy), DisplayName = "LesserThanOperator")]
        [DataRow(Operators.LesserThanOrEqual, typeof(LesserThanOrEqualOperatorEvalStrategy), DisplayName = "LesserThanOrEqualOperator")]
        public void OperatorEvalStrategyFactory_GetOperatorEvalStrategy_GivenOperator_ReturnsOperatorEvalStrategy(Operators @operator, Type type)
        {
            // Arrange
            Operators expectedOperator = @operator;
            Type expectedType = type;

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Act
            IOperatorEvalStrategy actual = sut.GetOperatorEvalStrategy(expectedOperator);

            // Assert
            Assert.IsInstanceOfType(actual, expectedType);
        }

        [TestMethod]
        public void OperatorEvalStrategyFactory_GetOperatorEvalStrategy_GivenUnknownOperator_ThrowsNotSupportedException()
        {
            // Arrange
            Operators expectedOperator = (Operators)(-1);

            OperatorEvalStrategyFactory sut = new OperatorEvalStrategyFactory();

            // Assert
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                // Act
                sut.GetOperatorEvalStrategy(expectedOperator);
            });
        }
    }
}