namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using Moq;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Tests.TestStubs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using FluentAssertions;

    public class OneToOneValueConditionNodeCompilerTests
    {
        [Fact]
        public void Compile_GivenValueConditionNodeAndParameterExpression_ReturnsCompiledLambda()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode = new ValueConditionNode<ConditionType>(
                DataTypes.String,
                ConditionType.IsoCountryCode,
                Operators.Equal,
                "ES");
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
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.Equal, Multiplicities.OneToOne))
                .Returns(conditionExpressionBuilder);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(dataTypeConfiguration);

            OneToOneValueConditionNodeCompiler manyToManyValueConditionNodeCompiler =
                new OneToOneValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            var compiledValueConditionNode = manyToManyValueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression);

            // Assert
            compiledValueConditionNode.Should().NotBeNull();
            actualLeftExpression.Should().NotBeNull();
            actualLeftExpression.Type.Should().Be<string>();
            actualRightExpression.Should().NotBeNull();
            actualRightExpression.Type.Should().Be<string>();
        }

        [Fact]
        public void Compile_GivenValueConditionNodeWithNullOperandAndParameterExpression_ReturnsCompiledLambdaUsingDataTypeDefault()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode = new ValueConditionNode<ConditionType>(
                DataTypes.Integer,
                ConditionType.NumberOfSales,
                Operators.Equal,
                null);
            ParameterExpression parameterExpression = Expression.Parameter(typeof(IDictionary<ConditionType, object>));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0);
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
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.Equal, Multiplicities.OneToOne))
                .Returns(conditionExpressionBuilder);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(dataTypeConfiguration);

            OneToOneValueConditionNodeCompiler oneToOneValueConditionNodeCompiler =
                new OneToOneValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            var compiledValueConditionNode = oneToOneValueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression);

            // Assert
            compiledValueConditionNode.Should().NotBeNull();
            actualLeftExpression.Should().NotBeNull();
            actualLeftExpression.Type.Should().Be<int>();
            actualRightExpression.Should().NotBeNull();
            actualRightExpression.Type.Should().Be<int>();
        }

        [Fact]
        public void Compile_GivenValueConditionNodeWithWrongTypeOperandAndParameterExpression_ThrowsArgumentException()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode = new ValueConditionNode<ConditionType>(
                DataTypes.Integer,
                ConditionType.NumberOfSales,
                Operators.Equal,
                "abc");
            ParameterExpression parameterExpression = Expression.Parameter(typeof(IDictionary<ConditionType, object>));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0);
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
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.Equal, Multiplicities.OneToOne))
                .Returns(conditionExpressionBuilder);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(dataTypeConfiguration);

            OneToOneValueConditionNodeCompiler oneToOneValueConditionNodeCompiler =
                new OneToOneValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => oneToOneValueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be("operand");
        }
    }
}
