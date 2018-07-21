using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rules.Framework.Core;
using Rules.Framework.Core.ConditionNodes;
using Rules.Framework.Evaluation.ValueEvaluation;
using Rules.Framework.Tests.TestStubs;

namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    [TestClass]
    public class DeferredEvalTests
    {
        [TestMethod]
        public void DeferredEval_GetDeferredEvalFor_GivenBooleanConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            BooleanConditionNode<ConditionType> conditionNode = new BooleanConditionNode<ConditionType>(ConditionType.IsVip, Operators.NotEqual, true);

            Mock<IOperatorEvalStrategy> mockOperatorEvalStrategy = new Mock<IOperatorEvalStrategy>();
            mockOperatorEvalStrategy.Setup(x => x.Eval(It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(true);

            Mock<IOperatorEvalStrategyFactory> mockOperatorEvalStrategyFactory = new Mock<IOperatorEvalStrategyFactory>();
            mockOperatorEvalStrategyFactory.Setup(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsVip,
                    Value = false
                }
            };

            DeferredEval sut = new DeferredEval(mockOperatorEvalStrategyFactory.Object);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            Assert.IsTrue(actualEvalResult);

            mockOperatorEvalStrategyFactory.Verify(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.Eval(It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [TestMethod]
        public void DeferredEval_GetDeferredEvalFor_GivenDecimalConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            DecimalConditionNode<ConditionType> conditionNode = new DecimalConditionNode<ConditionType>(ConditionType.PluviosityRate, Operators.GreaterThan, 50);

            Mock<IOperatorEvalStrategy> mockOperatorEvalStrategy = new Mock<IOperatorEvalStrategy>();
            mockOperatorEvalStrategy.Setup(x => x.Eval(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(true);

            Mock<IOperatorEvalStrategyFactory> mockOperatorEvalStrategyFactory = new Mock<IOperatorEvalStrategyFactory>();
            mockOperatorEvalStrategyFactory.Setup(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.PluviosityRate,
                    Value = 78
                }
            };

            DeferredEval sut = new DeferredEval(mockOperatorEvalStrategyFactory.Object);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            Assert.IsTrue(actualEvalResult);

            mockOperatorEvalStrategyFactory.Verify(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.Eval(It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once());
        }

        [TestMethod]
        public void DeferredEval_GetDeferredEvalFor_GivenIntegerConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            IntegerConditionNode<ConditionType> conditionNode = new IntegerConditionNode<ConditionType>(ConditionType.NumberOfSales, Operators.GreaterThan, 1000);

            Mock<IOperatorEvalStrategy> mockOperatorEvalStrategy = new Mock<IOperatorEvalStrategy>();
            mockOperatorEvalStrategy.Setup(x => x.Eval(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);

            Mock<IOperatorEvalStrategyFactory> mockOperatorEvalStrategyFactory = new Mock<IOperatorEvalStrategyFactory>();
            mockOperatorEvalStrategyFactory.Setup(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.NumberOfSales,
                    Value = 2300
                }
            };

            DeferredEval sut = new DeferredEval(mockOperatorEvalStrategyFactory.Object);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            Assert.IsTrue(actualEvalResult);

            mockOperatorEvalStrategyFactory.Verify(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.Eval(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void DeferredEval_GetDeferredEvalFor_GivenStringConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            StringConditionNode<ConditionType> conditionNode = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "EUR");

            Mock<IOperatorEvalStrategy> mockOperatorEvalStrategy = new Mock<IOperatorEvalStrategy>();
            mockOperatorEvalStrategy.Setup(x => x.Eval(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            Mock<IOperatorEvalStrategyFactory> mockOperatorEvalStrategyFactory = new Mock<IOperatorEvalStrategyFactory>();
            mockOperatorEvalStrategyFactory.Setup(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "EUR"
                }
            };

            DeferredEval sut = new DeferredEval(mockOperatorEvalStrategyFactory.Object);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            Assert.IsTrue(actualEvalResult);

            mockOperatorEvalStrategyFactory.Verify(x => x.GetOperatorEvalStrategy(It.IsAny<Operators>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.Eval(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void DeferredEval_GetDeferredEvalFor_GivenUnknownConditionNodeType_ThrowsNotSupportedException()
        {
            // Arrange
            Mock<IValueConditionNode<ConditionType>> mockValueConditionNode = new Mock<IValueConditionNode<ConditionType>>();

            Mock<IOperatorEvalStrategyFactory> mockOperatorEvalStrategyFactory = new Mock<IOperatorEvalStrategyFactory>();

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "EUR"
                }
            };

            DeferredEval sut = new DeferredEval(mockOperatorEvalStrategyFactory.Object);

            // Assert
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                // Act
                sut.GetDeferredEvalFor(mockValueConditionNode.Object);
            });
        }
    }
}