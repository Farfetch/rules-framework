namespace Rules.Framework.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ExpressionBuilderTests
    {
        [Fact]
        public void Build_GivenExpressionConfiguration_ReturnsExpressionResultWithBuiltExpression()
        {
            // Arrange
            var testParameter = Expression.Parameter(typeof(int), "input");
            var testVariable = Expression.Variable(typeof(int), "result");
            var testReturnLabelTarget = Expression.Label(typeof(int));

            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = "a",
                Expressions = new List<Expression>
                {
                    Expression.Assign(testVariable, testParameter),
                    Expression.Return(testReturnLabelTarget, testVariable),
                },
                LabelTargets = new Dictionary<string, LabelTarget>(),
                Parameters = new Dictionary<string, ParameterExpression> { { "input", testParameter } },
                ReturnDefaultValue = 0,
                ReturnLabelTarget = testReturnLabelTarget,
                ReturnType = typeof(int),
                Variables = new Dictionary<string, ParameterExpression> { { "result", testVariable } },
            };

            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var configuredExpressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var result = configuredExpressionBuilder.Build();

            // Assert
            result.Should().NotBeNull();
            result.ExpressionName.Should().Be("a");
            result.Implementation.Should().NotBeNull();
            result.Parameters.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expressionConfiguration.Parameters.Values);
            result.ReturnType.Should().Be(typeof(int));

            var compiledExpression = Expression.Lambda<Func<int, int>>(result.Implementation, result.Parameters).Compile();
            var compiledExpressionResult = compiledExpression.Invoke(4);
            compiledExpressionResult.Should().Be(4);

            configuredExpressionBuilder.ExpressionConfiguration.Should().BeSameAs(expressionConfiguration);
        }

        [Fact]
        public void HavingReturn_GivenNotNullTypeAndDefaultValue_ReturnsExpressionImplementationBuilder()
        {
            // Arrange
            var type = typeof(object);
            object defaultValue = null;

            var expressionName = "TestExpression";
            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = expressionName,
            };
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.HavingReturn(type, defaultValue);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionConfiguration.ReturnDefaultValue.Should().Be(defaultValue);
            expressionConfiguration.ReturnType.Should().Be(type);
            expressionConfiguration.ReturnLabelTarget.Should().NotBeNull();
            expressionConfiguration.ReturnLabelTarget.Name.Should().Contain(expressionName);
        }

        [Fact]
        public void HavingReturn_GivenNullTypeAndDefaultValue_ReturnsExpressionImplementationBuilder()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var action = FluentActions.Invoking(() => expressionBuilder.HavingReturn(null, null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("type");
        }

        [Fact]
        public void HavingReturn_GivenTypeAsGenericParameterAndDefaultValue_ReturnsExpressionImplementationBuilder()
        {
            // Arrange
            object defaultValue = null;

            var expressionName = "TestExpression";
            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = expressionName,
            };
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.HavingReturn<object>(defaultValue);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionConfiguration.ReturnDefaultValue.Should().Be(defaultValue);
            expressionConfiguration.ReturnType.Should().Be(typeof(object));
            expressionConfiguration.ReturnLabelTarget.Should().NotBeNull();
            expressionConfiguration.ReturnLabelTarget.Name.Should().Contain(expressionName);
        }

        [Fact]
        public void NewExpression_GivenNonNullEmptyOrWhitespaceName_ReturnsNewNamedExpressionBuilder()
        {
            // Arrange
            var expressionName = "TestExpression";

            // Act
            var actual = ExpressionBuilder.NewExpression(expressionName);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionBuilder>();

            var expressionBuilder = actual as ExpressionBuilder;
            expressionBuilder.ExpressionConfiguration.ExpressionName.Should().Be(expressionName);
        }

        [Fact]
        public void NewExpression_GivenNullEmptyOrWhitespaceName_ThrowsArgumentException()
        {
            // Arrange
            var expressionName = "";

            // Act
            var action = FluentActions.Invoking(() => ExpressionBuilder.NewExpression(expressionName));

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("name");
        }

        [Fact]
        public void SetImplementation_GivenNonNullBuilderAction_ReturnsConfiguredExpressionBuilder()
        {
            // Arrange
            var testParameter = Expression.Parameter(typeof(int), "input");
            var testReturnLabelTarget = Expression.Label(typeof(int));

            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = "a",
                Parameters = new Dictionary<string, ParameterExpression> { { "input", testParameter } },
                ReturnDefaultValue = 0,
                ReturnLabelTarget = testReturnLabelTarget,
                ReturnType = typeof(int),
            };

            var testExpressions = new List<Expression>()
            {
                Expression.Constant(1),
            };
            var testVariables = new Dictionary<string, ParameterExpression>()
            {
                { "result", Expression.Variable(typeof(int), "result") },
            };
            var testLabelTargets = new Dictionary<string, LabelTarget>()
            {
                { "Return", Expression.Label(typeof(int)) },
            };
            var executedBuilderAction = false;

            Action<IExpressionBlockBuilder> builderAction = b =>
            {
                executedBuilderAction = true;
            };

            var implementationExpressionBuilder = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(implementationExpressionBuilder)
                .SetupGet(x => x.LabelTargets)
                .Returns(testLabelTargets);
            Mock.Get(implementationExpressionBuilder)
                .SetupGet(x => x.Variables)
                .Returns(testVariables);
            Mock.Get(implementationExpressionBuilder)
                .SetupGet(x => x.Expressions)
                .Returns(testExpressions);

            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionBlockBuilder(It.IsAny<string>(), It.IsAny<IExpressionBlockBuilder>(), It.IsAny<ExpressionConfiguration>()))
                .Returns(implementationExpressionBuilder);

            var expressionBuilder = new ExpressionBuilder(
                expressionConfiguration,
                expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.SetImplementation(builderAction);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            executedBuilderAction.Should().BeTrue();
            expressionConfiguration.LabelTargets.Should().NotBeNull()
                .And.BeSameAs(testLabelTargets);
            expressionConfiguration.Variables.Should().NotBeNull()
                .And.BeSameAs(testVariables);
            expressionConfiguration.Expressions.Should().NotBeNull()
                .And.BeSameAs(testExpressions);
        }

        [Fact]
        public void SetImplementation_GivenNullBuilderAction_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();

            Action<IExpressionBlockBuilder> builderAction = null;

            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(
                expressionConfiguration,
                expressionBuilderFactory);

            // Act
            var action = FluentActions.Invoking(() => expressionBuilder.SetImplementation(builderAction));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("builder");
        }

        [Fact]
        public void WithoutParameters_NoConditions_ReturnsExpressionReturnBuilder()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.WithoutParameters();

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionConfiguration.Parameters.Should().NotBeNull()
                .And.BeEmpty();
        }

        [Fact]
        public void WithParameters_GivenNotNullBuilderAction_ReturnsExpressionReturnBuilder()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var parameterExpressions = new Dictionary<string, ParameterExpression>();
            var expressionParametersConfiguration = Mock.Of<IExpressionParametersConfiguration>();
            Mock.Get(expressionParametersConfiguration)
                .Setup(x => x.CreateParameter<It.IsAnyType>(It.IsAny<string>()))
                .Returns((IInvocation i) =>
                {
                    var parameterExpression = Expression.Parameter(i.Method.GetGenericArguments()[0], (string)i.Arguments[0]);
                    parameterExpressions.Add(parameterExpression.Name, parameterExpression);
                    return parameterExpression;
                });
            Mock.Get(expressionParametersConfiguration)
                .SetupGet(x => x.Parameters)
                .Returns(parameterExpressions);
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionParametersConfiguration())
                .Returns(expressionParametersConfiguration);

            var expressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.WithParameters(c =>
            {
                c.CreateParameter<object>("testParameter");
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionConfiguration.Parameters.Should().ContainKey("testParameter");
        }

        [Fact]
        public void WithParameters_GivenNullBuilderAction_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionConfiguration, expressionBuilderFactory);

            // Act
            var action = FluentActions.Invoking(() => expressionBuilder.WithParameters(null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("parametersConfigurationAction");
        }
    }
}