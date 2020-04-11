namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class ConditionsEvalEngineTests
    {
        [Fact]
        public void Eval_GivenComposedConditionNodeWithAndOperator_EvalsAndReturnsResult()
        {
            // Arrange
            BooleanConditionNode<ConditionType> condition1 = new BooleanConditionNode<ConditionType>(ConditionType.IsVip, Operators.Equal, true);
            StringConditionNode<ConditionType> condition2 = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            ComposedConditionNode<ConditionType> composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.And,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            IEnumerable<Condition<ConditionType>> conditions = new[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "SGD"
                },
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsVip,
                    Value = true
                }
            };

            Mock<IDeferredEval> mockDeferredEval = new Mock<IDeferredEval>();
            mockDeferredEval.SetupSequence(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>()))
                .Returns(() =>
                {
                    return (c) => true;
                })
                .Returns(() =>
                {
                    return (c) => true;
                })
                .Throws(new NotImplementedException("Shouldn't have gotten any more deferred evals."));

            ConditionsEvalEngine<ConditionType> sut = new ConditionsEvalEngine<ConditionType>(mockDeferredEval.Object);

            // Act
            bool actual = sut.Eval(composedConditionNode, conditions);

            // Assert
            actual.Should().BeTrue();

            mockDeferredEval.Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>()), Times.Exactly(2));
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithEvalOperator_ThrowsNotSupportedException()
        {
            // Arrange
            BooleanConditionNode<ConditionType> condition1 = new BooleanConditionNode<ConditionType>(ConditionType.IsVip, Operators.Equal, true);
            StringConditionNode<ConditionType> condition2 = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            ComposedConditionNode<ConditionType> composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Eval,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            IEnumerable<Condition<ConditionType>> conditions = new[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "SGD"
                },
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsVip,
                    Value = true
                }
            };

            Mock<IDeferredEval> mockDeferredEval = new Mock<IDeferredEval>();

            ConditionsEvalEngine<ConditionType> sut = new ConditionsEvalEngine<ConditionType>(mockDeferredEval.Object);

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
            mockDeferredEval.Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>()), Times.Never());
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithOrOperator_EvalsAndReturnsResult()
        {
            // Arrange
            BooleanConditionNode<ConditionType> condition1 = new BooleanConditionNode<ConditionType>(ConditionType.IsVip, Operators.Equal, true);
            StringConditionNode<ConditionType> condition2 = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            ComposedConditionNode<ConditionType> composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Or,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            IEnumerable<Condition<ConditionType>> conditions = new[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "SGD"
                },
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsVip,
                    Value = true
                }
            };

            Mock<IDeferredEval> mockDeferredEval = new Mock<IDeferredEval>();
            mockDeferredEval.SetupSequence(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>()))
                .Returns(() =>
                {
                    return (c) => false;
                })
                .Returns(() =>
                {
                    return (c) => true;
                })
                .Throws(new NotImplementedException("Shouldn't have gotten any more deferred evals."));

            ConditionsEvalEngine<ConditionType> sut = new ConditionsEvalEngine<ConditionType>(mockDeferredEval.Object);

            // Act
            bool actual = sut.Eval(composedConditionNode, conditions);

            // Assert
            actual.Should().BeTrue();

            mockDeferredEval.Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>()), Times.Exactly(2));
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithUnknownConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            Mock<IConditionNode<ConditionType>> mockConditionNode = new Mock<IConditionNode<ConditionType>>();

            IEnumerable<Condition<ConditionType>> conditions = new[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "SGD"
                },
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsVip,
                    Value = true
                }
            };

            Mock<IDeferredEval> mockDeferredEval = new Mock<IDeferredEval>();

            ConditionsEvalEngine<ConditionType> sut = new ConditionsEvalEngine<ConditionType>(mockDeferredEval.Object);

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(mockConditionNode.Object, conditions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Unsupported condition node: '{mockConditionNode.Object.GetType().Name}'.");

            mockDeferredEval.Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>()), Times.Never());
        }
    }
}