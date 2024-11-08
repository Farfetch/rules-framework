namespace Rules.Framework.Tests.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
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
            var condition1 = new ValueConditionNode(DataTypes.Boolean, ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.And,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Search,
                ExcludeRulesWithoutSearchConditions = true
            };

            var mockDeferredEval = new Mock<IDeferredEval>();
            mockDeferredEval.SetupSequence(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)))
                .Returns(() =>
                {
                    return (_) => false;
                })
                .Returns(() =>
                {
                    return (_) => true;
                })
                .Throws(new NotImplementedException("Shouldn't have gotten any more deferred evals."));
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();

            var sut = new InterpretedConditionsEvalEngine(mockDeferredEval.Object, conditionsTreeAnalyzer);

            // Act
            var actual = sut.Eval(composedConditionNode, conditions, evaluationOptions);

            // Assert
            actual.Should().BeFalse();

            mockDeferredEval.Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Exactly(0));
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithAndOperatorWithExactMatch_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode(DataTypes.Boolean, ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Eval,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var deferredEval = Mock.Of<IDeferredEval>();
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();

            var sut = new InterpretedConditionsEvalEngine(deferredEval, conditionsTreeAnalyzer);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions, evaluationOptions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
            Mock.Get(deferredEval)
                .Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Never());
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithEvalOperator_ThrowsNotSupportedException()
        {
            // Arrange
            var condition1 = new ValueConditionNode(DataTypes.Boolean, ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Eval,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var deferredEval = Mock.Of<IDeferredEval>();
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();

            var sut = new InterpretedConditionsEvalEngine(deferredEval, conditionsTreeAnalyzer);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions, evaluationOptions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
            Mock.Get(deferredEval)
                .Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Never());
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithOrOperatorWithExactMatch_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode(DataTypes.Boolean, ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Or,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, object>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var deferredEval = Mock.Of<IDeferredEval>();
            Mock.Get(deferredEval)
                .SetupSequence(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)))
                .Returns(() =>
                {
                    return (_) => true;
                })
                .Returns(() =>
                {
                    return (_) => true;
                })
                .Throws(new NotImplementedException("Shouldn't have gotten any more deferred evals."));
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();

            var sut = new InterpretedConditionsEvalEngine(deferredEval, conditionsTreeAnalyzer);

            // Act
            var actual = sut.Eval(composedConditionNode, conditions, evaluationOptions);

            // Assert
            actual.Should().BeTrue();

            Mock.Get(deferredEval)
                .Verify(x => x.GetDeferredEvalFor(It.IsAny<IValueConditionNode>(), It.Is<MatchModes>(mm => mm == MatchModes.Exact)), Times.Exactly(2));
        }
    }
}