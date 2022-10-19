namespace Rules.Framework.Tests.Evaluation.ValueEvaluation.Dispatchers
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation;
    using Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers;
    using Xunit;

    public class OneToOneConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsInteger_ConvertsIntegersToStringsEvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = 2;
            Operators @operator = Operators.In;

            IOneToOneOperatorEvalStrategy oneToOneOperatorEvalStrategy = Mock.Of<IOneToOneOperatorEvalStrategy>();
            Mock.Get(oneToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<object>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToOneOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(oneToOneOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            OneToOneConditionEvalDispatcher oneToOneConditionEvalDispatcher = new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            bool result = oneToOneConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsStringCollection_ThrowsArgumentExceptionForRightOperand()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = new[] { "str3", "str4" };
            Operators @operator = Operators.In;

            IOneToOneOperatorEvalStrategy oneToOneOperatorEvalStrategy = Mock.Of<IOneToOneOperatorEvalStrategy>();
            Mock.Get(oneToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<object>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToOneOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(oneToOneOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            OneToOneConditionEvalDispatcher oneToOneConditionEvalDispatcher = new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => oneToOneConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(rightOperand));
            argumentException.Message.Should().Contain(nameof(String));
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsInteger_ThrowsArgumentExceptionForLeftOperand()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = 1;
            Operators @operator = Operators.In;

            IOneToOneOperatorEvalStrategy oneToOneOperatorEvalStrategy = Mock.Of<IOneToOneOperatorEvalStrategy>();
            Mock.Get(oneToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<object>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToOneOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(oneToOneOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            OneToOneConditionEvalDispatcher oneToOneConditionEvalDispatcher = new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => oneToOneConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(leftOperand));
            argumentException.Message.Should().Contain(nameof(String));
        }
    }
}