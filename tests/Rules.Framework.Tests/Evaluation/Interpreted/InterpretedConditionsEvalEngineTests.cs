namespace Rules.Framework.Tests.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Interpreted;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class InterpretedConditionsEvalEngineTests
    {
        [Fact]
        public void Eval_GivenComposedConditionNodeWithAndOperatorAndMissingConditionWithSearchMode_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.Equal, true);
            var condition2 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.And,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            var conditions = new Dictionary<ConditionType, object>
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsVip,
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Search,
                ExcludeRulesWithoutSearchConditions = true
            };

            var mockDeferredEval = new Mock<IDeferredEval>();
            mockDeferredEval.SetupSequence(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)))
                .Returns(() =>
                {
                    return (_) => false;
                })
                .Returns(() =>
                {
                    return (_) => true;
                })
                .Throws(new NotImplementedException("Shouldn't have gotten any more deferred evals."));
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();

            var sut = new InterpretedConditionsEvalEngine<ConditionType>(mockDeferredEval.Object, conditionsTreeAnalyzer);

            // Act
            var actual = sut.Eval(composedConditionNode, conditions, evaluationOptions);

            // Assert
            actual.Should().BeFalse();

            mockDeferredEval.Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Exactly(0));
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithAndOperatorWithExactMatch_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.Equal, true);
            var condition2 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Eval,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            var conditions = new Dictionary<ConditionType, object>
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsVip,
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var deferredEval = Mock.Of<IDeferredEval>();
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();

            var sut = new InterpretedConditionsEvalEngine<ConditionType>(deferredEval, conditionsTreeAnalyzer);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions, evaluationOptions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
            Mock.Get(deferredEval)
                .Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Never());
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithEvalOperator_ThrowsNotSupportedException()
        {
            // Arrange
            var condition1 = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.Equal, true);
            var condition2 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Eval,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            var conditions = new Dictionary<ConditionType, object>
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsVip,
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var deferredEval = Mock.Of<IDeferredEval>();
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();

            var sut = new InterpretedConditionsEvalEngine<ConditionType>(deferredEval, conditionsTreeAnalyzer);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions, evaluationOptions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
            Mock.Get(deferredEval)
                .Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Never());
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithOrOperatorWithExactMatch_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.Equal, true);
            var condition2 = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode<ConditionType>(
                LogicalOperators.Or,
                new IConditionNode<ConditionType>[] { condition1, condition2 });

            var conditions = new Dictionary<ConditionType, object>
            {
                {
                    ConditionType.IsoCurrency,
                    "SGD"
                },
                {
                    ConditionType.IsVip,
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var deferredEval = Mock.Of<IDeferredEval>();
            Mock.Get(deferredEval)
                .SetupSequence(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)))
                .Returns(() =>
                {
                    return (_) => true;
                })
                .Returns(() =>
                {
                    return (_) => true;
                })
                .Throws(new NotImplementedException("Shouldn't have gotten any more deferred evals."));
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();

            var sut = new InterpretedConditionsEvalEngine<ConditionType>(deferredEval, conditionsTreeAnalyzer);

            // Act
            var actual = sut.Eval(composedConditionNode, conditions, evaluationOptions);

            // Assert
            actual.Should().BeTrue();

            Mock.Get(deferredEval)
                .Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode<ConditionType>>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Exactly(2));
        }
    }
}