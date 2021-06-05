namespace Rules.Framework.Tests.Evaluation.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;
    using Xunit;

    public class OneToManyConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsStringCollection_ConvertsIntegerToStringEvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = new[] { "str3", "str4" };
            Operators @operator = Operators.In;

            IOneToManyOperatorEvalStrategy manyToOneOperatorEvalStrategy = Mock.Of<IOneToManyOperatorEvalStrategy>();
            Mock.Get(manyToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToOneOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            OneToManyConditionEvalDispatcher oneToManyConditionEvalDispatcher = new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            bool result = oneToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsInteger_ThrowsArgumentExceptionForLeftOperand()
        {
            // Arrange
            DataTypes dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = 1;
            Operators @operator = Operators.In;

            IOneToManyOperatorEvalStrategy oneToManyOperatorEvalStrategy = Mock.Of<IOneToManyOperatorEvalStrategy>();
            Mock.Get(oneToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(oneToManyOperatorEvalStrategy);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            OneToManyConditionEvalDispatcher oneToManyConditionEvalDispatcher = new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => oneToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(leftOperand));
            argumentException.Message.Should().Contain(nameof(String));
        }
    }
}