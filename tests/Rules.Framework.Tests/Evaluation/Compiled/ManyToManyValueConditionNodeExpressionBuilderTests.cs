namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using DiffPlex.DiffBuilder;
    using ExpressionDebugger;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ManyToManyValueConditionNodeExpressionBuilderTests
    {
        [Fact]
        public void Build_GivenLeftHandOperatorRightHandAndDataTypeConfiguration_ReturnsExpression()
        {
            // Arrange
            string expectedScript;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rules.Framework.Tests.Evaluation.Compiled.ManyToManyValueConditionNodeExpressionBuilderTests.GoldenFile1.csx"))
            using (var streamReader = new StreamReader(stream))
            {
                expectedScript = streamReader.ReadToEnd();
            }
            Expression actualLeftExpression = null;
            Expression actualRightExpression = null;

            var conditionExpression = Expression.AndAlso(Expression.Constant(true, typeof(bool)), Expression.Constant(true, typeof(bool))); // For testing purposes, does not need to stay true to the scenario
            var conditionExpressionBuilder = Mock.Of<IConditionExpressionBuilder>();
            Mock.Get(conditionExpressionBuilder)
                .Setup(x => x.BuildConditionExpression(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildConditionExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildConditionExpressionArgs>((b, args) =>
                {
                    actualLeftExpression = args.LeftHandOperand;
                    actualRightExpression = args.RightHandOperand;
                })
                .Returns(conditionExpression);
            var conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();
            Mock.Get(conditionExpressionBuilderProvider)
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.Contains, Multiplicities.ManyToMany))
                .Returns(conditionExpressionBuilder);

            var manyToManyValueConditionNodeCompiler =
                new ManyToManyValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider);

            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestDummy")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftOperand", typeof(object));
                    p.CreateParameter("rightOperand", typeof(object));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var resultVariableExpression = builder.CreateVariable("result", typeof(bool));
                    var args = new BuildValueConditionNodeExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null),
                        LeftOperandVariableExpression = builder.GetParameter("leftOperand"),
                        Operator = Operators.Contains,
                        ResultVariableExpression = resultVariableExpression,
                        RightOperandVariableExpression = builder.GetParameter("rightOperand"),
                    };

                    manyToManyValueConditionNodeCompiler.Build(builder, args);

                    builder.Return(resultVariableExpression);
                })
                .Build();

            // Assert
            expressionResult.Should().NotBeNull();
            expressionResult.Implementation.Should().NotBeNull();

            // Purely for the effect of comparing the generated lambda expression code with the
            // golden file.
            var lambdaExpression = Expression.Lambda<Func<object, object, bool>>(expressionResult.Implementation, expressionResult.Parameters);
            var actualScript = lambdaExpression.ToScript();
            var diffResult = SideBySideDiffBuilder.Diff(expectedScript, actualScript, ignoreWhiteSpace: true);
            diffResult.NewText.HasDifferences.Should().BeFalse();
            Func<object, object, bool> compiledLambdaExpression = null;
            FluentActions.Invoking(() => compiledLambdaExpression = lambdaExpression.Compile())
                .Should()
                .NotThrow("expression should be compilable");
            FluentActions.Invoking(() => compiledLambdaExpression.Invoke(null, new string[] { "dummy" }))
                .Should()
                .NotThrow("compiled expression should be executable");
            FluentActions.Invoking(() => compiledLambdaExpression.Invoke(new string[] { "abc" }, new string[] { "dummy" }))
                .Should()
                .NotThrow("compiled expression should be able to deal with null left operand");
            actualLeftExpression.Should().NotBeNull();
            actualLeftExpression.Type.Should().BeAssignableTo<IEnumerable<string>>();
            actualRightExpression.Should().NotBeNull();
            actualRightExpression.Type.Should().BeAssignableTo<IEnumerable<string>>();
        }
    }
}