namespace Rules.Framework.Tests.Evaluation.Interpreted
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
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
            var conditionNode = new ValueConditionNode(DataTypes.Boolean, ConditionNames.IsVip.ToString(), Operators.NotEqual, true);

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsVip.ToString(), false }
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
        public void GetDeferredEvalFor_GivenDecimalConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            var conditionNode = new ValueConditionNode(DataTypes.Decimal, ConditionNames.PluviosityRate.ToString(), Operators.GreaterThan, 50);

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.PluviosityRate.ToString(), 78 }
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
            var conditionNode = new ValueConditionNode(DataTypes.Integer, ConditionNames.NumberOfSales.ToString(), Operators.GreaterThan, 1000);

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.NumberOfSales.ToString(), 2300 }
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
            var conditionNode = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.Equal, "EUR");

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCurrency.ToString(), "EUR" }
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
            var conditionNode = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.Equal, "EUR");

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCountryCode.ToString(), "PT" }
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
            var conditionNode = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.Equal, "EUR");

            var mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(false);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCountryCode.ToString(), "PT" }
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
    }
}