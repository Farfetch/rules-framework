namespace Rules.Framework.Tests.Evaluation.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;
    using Xunit;

    public class ManyToManyConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsStringCollection_ThrowsArgumentExceptionForLeftOperand()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = new[] { "str3", "str4" };
            Operators @operator = Operators.In;

            IManyToManyOperatorEvalStrategy manyToManyOperatorEvalStrategy = Mock.Of<IManyToManyOperatorEvalStrategy>();
            Mock.Get(manyToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToManyOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            ManyToManyConditionEvalDispatcher manyToManyConditionEvalDispatcher = new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => manyToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(leftOperand));
            argumentException.Message.Should().Contain(nameof(IEnumerable));
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsInteger_ThrowsArgumentExceptionForRightOperand()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = 1;
            Operators @operator = Operators.In;

            IManyToManyOperatorEvalStrategy manyToManyOperatorEvalStrategy = Mock.Of<IManyToManyOperatorEvalStrategy>();
            Mock.Get(manyToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToManyOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            ManyToManyConditionEvalDispatcher manyToManyConditionEvalDispatcher = new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => manyToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(rightOperand));
            argumentException.Message.Should().Contain(nameof(IEnumerable));
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsStringCollection_EvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = new[] { "str3", "str4" };
            Operators @operator = Operators.In;

            IManyToManyOperatorEvalStrategy manyToManyOperatorEvalStrategy = Mock.Of<IManyToManyOperatorEvalStrategy>();
            Mock.Get(manyToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToManyOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            ManyToManyConditionEvalDispatcher manyToManyConditionEvalDispatcher = new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            bool result = manyToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }
    }
}