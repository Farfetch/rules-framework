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
    using Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class DeferredEvalTests
    {
        [Fact]
        public void GetDeferredEvalFor_GivenBooleanConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            var conditionNode = new ValueConditionNode<ConditionType>(DataTypes.Boolean, ConditionType.IsVip, Operators.NotEqual, true);

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<ConditionType, object>
            {
                { ConditionType.IsVip, false }
            };

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenDecimalConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            var conditionNode = new ValueConditionNode<ConditionType>(DataTypes.Decimal, ConditionType.PluviosityRate, Operators.GreaterThan, 50);

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<ConditionType, object>
            {
                { ConditionType.PluviosityRate, 78 }
            };

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            var actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenIntegerConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            var conditionNode = new ValueConditionNode<ConditionType>(DataTypes.Integer, ConditionType.NumberOfSales, Operators.GreaterThan, 1000);

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<ConditionType, object>
            {
                { ConditionType.NumberOfSales, 2300 }
            };

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            var actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenStringConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            var conditionNode = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.Equal, "EUR");

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<ConditionType, object>
            {
                { ConditionType.IsoCurrency, "EUR" }
            };

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            var actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenStringConditionNodeWithNoConditionSuppliedAndRulesEngineConfiguredToDiscardWhenMissing_ReturnsFuncThatEvalsFalse()
        {
            // Arrange
            var conditionNode = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.Equal, "EUR");

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<ConditionType, object>
            {
                { ConditionType.IsoCountryCode, "PT" }
            };

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.MissingConditionBehavior = MissingConditionBehaviors.Discard;

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            var actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeFalse();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Never());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Never());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenStringConditionNodeWithNoConditionSuppliedAndRulesEngineConfiguredToUseDataTypeDefaultWhenMissing_ReturnsFuncThatEvalsFalse()
        {
            // Arrange
            var conditionNode = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCurrency, Operators.Equal, "EUR");

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(false);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<ConditionType, object>
            {
                { ConditionType.IsoCountryCode, "PT" }
            };

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.MissingConditionBehavior = MissingConditionBehaviors.UseDataTypeDefault;

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            var actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeFalse();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenUnknownConditionNodeType_ThrowsNotSupportedException()
        {
            // Arrange
            var mockValueConditionNode = new Mock<IValueConditionNode<ConditionType>>();

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();

            var matchMode = MatchModes.Exact;

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetDeferredEvalFor(mockValueConditionNode.Object, matchMode));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Unsupported value condition node: '{mockValueConditionNode.Object.GetType().Name}'.");
        }
    }
}