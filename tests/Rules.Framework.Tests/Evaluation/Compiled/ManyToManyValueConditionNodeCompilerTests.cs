namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Tests.TestStubs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ManyToManyValueConditionNodeCompilerTests
    {
        [Fact]
        public void Compile_GivenValueConditionNodeAndParameterExpression_ReturnsCompiledLambda()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode = new ValueConditionNode<ConditionType>(
                DataTypes.String,
                ConditionType.IsoCountryCode,
                Operators.Contains,
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
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.Contains, Multiplicities.ManyToMany))
                .Returns(conditionExpressionBuilder);
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(dataTypeConfiguration);

            ManyToManyValueConditionNodeCompiler manyToManyValueConditionNodeCompiler =
                new ManyToManyValueConditionNodeCompiler(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            var compiledValueConditionNode = manyToManyValueConditionNodeCompiler.Compile(valueConditionNode, parameterExpression);

            // Assert
            compiledValueConditionNode.Should().NotBeNull();
            actualLeftExpression.Should().NotBeNull();
            actualLeftExpression.Type.Should().BeAssignableTo<IEnumerable<string>>();
            actualRightExpression.Should().NotBeNull();
            actualRightExpression.Type.Should().BeAssignableTo<IEnumerable<string>>();
        }
    }
}
