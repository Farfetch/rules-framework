namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using Moq;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Tests.TestStubs;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Xunit;
    using FluentAssertions;
    using System;

    public class OneToManyValueConditionNodeCompilerTests
    {
        [Fact]
        public void Compile_GivenValueConditionNodeAndParameterExpression_ReturnsCompiledLambda()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode = new ValueConditionNode<ConditionType>(
                DataTypes.String,
                ConditionType.IsoCountryCode,
                Operators.In,
                new[] { "PT", "ES" });
            ParameterExpression parameterExpression = Expression.Parameter(typeof(IDictionary<ConditionType, object>));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null);
            Expression actualLeftExpression = null;
            Expression actualRightExpression = null;

            Expression conditionExpression = Expression.Constant(true, typeof(bool)); // For testing purposes, does not need to stay true to the scenario
            IConditionExpressionBuilder conditionExpressionBuilder = Mock.Of<IConditionExpressionBuilder>();
            Mock.Get(conditionExpressionBuilder)
                .Setup(x => x.BuildConditionExpression(It.IsAny<Expression>(), It.IsAny<Expression>(), It.IsAny<DataTypeConfiguration>()))
                .Callback<Expression, Expression, DataTypeConfiguration>((e1, e2, dtc) =>
                {
                    actualLeftExpression = e1;
                    actualRightExpression = e2;
                })
                .Returns(conditionExpression);
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();
            Mock.Get(conditionExpressionBuilderProvider)
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.In, Multiplicities.OneToMany))
                .Returns(conditionExpressionBuilder);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(dataTypeConfiguration);

            OneToManyValueConditionNodeCompiler manyToManyValueConditionNodeCompiler =
                new OneToManyValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            var compiledValueConditionNode = manyToManyValueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression);

            // Assert
            compiledValueConditionNode.Should().NotBeNull();
            actualLeftExpression.Should().NotBeNull();
            actualLeftExpression.Type.Should().Be<string>();
            actualRightExpression.Should().NotBeNull();
            actualRightExpression.Type.Should().BeAssignableTo<IEnumerable<string>>();
        }

        [Fact]
        public void Compile_GivenValueConditionNodeWithValueNonArrayTypeAndParameterExpression_ThrowsArgumentException()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode = new ValueConditionNode<ConditionType>(
                DataTypes.String,
                ConditionType.IsoCountryCode,
                Operators.In,
                "PT");
            ParameterExpression parameterExpression = Expression.Parameter(typeof(IDictionary<ConditionType, object>));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null);
            Expression actualLeftExpression = null;
            Expression actualRightExpression = null;

            Expression conditionExpression = Expression.Constant(true, typeof(bool)); // For testing purposes, does not need to stay true to the scenario
            IConditionExpressionBuilder conditionExpressionBuilder = Mock.Of<IConditionExpressionBuilder>();
            Mock.Get(conditionExpressionBuilder)
                .Setup(x => x.BuildConditionExpression(It.IsAny<Expression>(), It.IsAny<Expression>(), It.IsAny<DataTypeConfiguration>()))
                .Callback<Expression, Expression, DataTypeConfiguration>((e1, e2, dtc) =>
                {
                    actualLeftExpression = e1;
                    actualRightExpression = e2;
                })
                .Returns(conditionExpression);
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();
            Mock.Get(conditionExpressionBuilderProvider)
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.In, Multiplicities.OneToMany))
                .Returns(conditionExpressionBuilder);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(dataTypeConfiguration);

            OneToManyValueConditionNodeCompiler manyToManyValueConditionNodeCompiler =
                new OneToManyValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => manyToManyValueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be("operand");
        }
    }
}
