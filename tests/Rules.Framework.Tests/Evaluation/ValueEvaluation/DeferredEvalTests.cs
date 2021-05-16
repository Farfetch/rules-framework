namespace Rules.Framework.Tests.Evaluation.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class DeferredEvalTests
    {
        [Fact]
        public void GetDeferredEvalFor_GivenBooleanConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            BooleanConditionNode<ConditionType> conditionNode = new BooleanConditionNode<ConditionType>(ConditionType.IsVip, Operators.NotEqual, true);

            Mock<IConditionEvalDispatcher> mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsVip,
                    Value = false
                }
            };

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
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
            DecimalConditionNode<ConditionType> conditionNode = new DecimalConditionNode<ConditionType>(ConditionType.PluviosityRate, Operators.GreaterThan, 50);

            Mock<IConditionEvalDispatcher> mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.PluviosityRate,
                    Value = 78
                }
            };

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenIntegerConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            IntegerConditionNode<ConditionType> conditionNode = new IntegerConditionNode<ConditionType>(ConditionType.NumberOfSales, Operators.GreaterThan, 1000);

            Mock<IConditionEvalDispatcher> mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.NumberOfSales,
                    Value = 2300
                }
            };

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenStringConditionNode_ReturnsFuncToEvalConditionsCollection()
        {
            // Arrange
            StringConditionNode<ConditionType> conditionNode = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "EUR");

            Mock<IConditionEvalDispatcher> mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "EUR"
                }
            };

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeTrue();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenStringConditionNodeWithNoConditionSuppliedAndRulesEngineConfiguredToDiscardWhenMissing_ReturnsFuncThatEvalsFalse()
        {
            // Arrange
            StringConditionNode<ConditionType> conditionNode = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "EUR");

            Mock<IConditionEvalDispatcher> mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(true);

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCountryCode,
                    Value = "PT"
                }
            };

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.MissingConditionBehavior = MissingConditionBehaviors.Discard;

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeFalse();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Never());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Never());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenStringConditionNodeWithNoConditionSuppliedAndRulesEngineConfiguredToUseDataTypeDefaultWhenMissing_ReturnsFuncThatEvalsFalse()
        {
            // Arrange
            StringConditionNode<ConditionType> conditionNode = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "EUR");

            Mock<IConditionEvalDispatcher> mockOperatorEvalStrategy = new Mock<IConditionEvalDispatcher>();
            mockOperatorEvalStrategy.Setup(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(false);

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()))
                .Returns(mockOperatorEvalStrategy.Object);

            IEnumerable<Condition<ConditionType>> conditions = new Condition<ConditionType>[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCountryCode,
                    Value = "PT"
                }
            };

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.MissingConditionBehavior = MissingConditionBehaviors.UseDataTypeDefault;

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            Func<IEnumerable<Condition<ConditionType>>, bool> actual = sut.GetDeferredEvalFor(conditionNode, matchMode);
            bool actualEvalResult = actual.Invoke(conditions);

            // Assert
            actualEvalResult.Should().BeFalse();

            mockConditionEvalDispatchProvider.Verify(x => x.GetEvalDispatcher(It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
            mockOperatorEvalStrategy.Verify(x => x.EvalDispatch(It.IsAny<DataTypes>(), It.IsAny<object>(), It.IsAny<Operators>(), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public void GetDeferredEvalFor_GivenUnknownConditionNodeType_ThrowsNotSupportedException()
        {
            // Arrange
            Mock<IValueConditionNode<ConditionType>> mockValueConditionNode = new Mock<IValueConditionNode<ConditionType>>();

            Mock<IConditionEvalDispatchProvider> mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatchProvider>();

            MatchModes matchMode = MatchModes.Exact;

            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DeferredEval sut = new DeferredEval(mockConditionEvalDispatchProvider.Object, rulesEngineOptions);

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => sut.GetDeferredEvalFor(mockValueConditionNode.Object, matchMode));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"Unsupported value condition node: '{mockValueConditionNode.Object.GetType().Name}'.");
        }
    }
}