namespace Rules.Framework.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ExpressionBlockBuilderTests
    {
        [Fact]
        public void AddExpression_GivenNotNullExpression_AddsExpressionToExpressionsList()
        {
            // Arrange
            var expression = Expression.Constant(1);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.AddExpression(expression);

            // Assert
            expressionBlockBuilder.Expressions.Should().HaveCount(1)
                .And.Contain(expression);
        }

        [Fact]
        public void AddExpression_GivenNullExpression_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.AddExpression(null));

            // Arrange
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("expression");
        }

        [Fact]
        public void AndAlso_GivenCollectionOfExpressionsWithMultipleExpressions_ReturnsExpression()
        {
            // Arrange
            var expressions = new Expression[]
            {
                Expression.Constant(true),
                Expression.Constant(true),
                Expression.Constant(true),
                Expression.Constant(true),
                Expression.Constant(true),
            };

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.AndAlso(expressions);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.AndAlso);

            var actualAsBinaryExpression = actual as BinaryExpression;
            actualAsBinaryExpression.Right.Should().BeAssignableTo<ConstantExpression>();
            actualAsBinaryExpression.Left.Should().BeAssignableTo<BinaryExpression>();
            var chainedExpressionsCount = 1;
            while (actualAsBinaryExpression.Left is BinaryExpression)
            {
                actualAsBinaryExpression.Right.Should().BeAssignableTo<ConstantExpression>();
                actualAsBinaryExpression = actualAsBinaryExpression.Left as BinaryExpression;
                chainedExpressionsCount++;
            }

            actualAsBinaryExpression.Left.Should().BeAssignableTo<ConstantExpression>();
            actualAsBinaryExpression.Right.Should().BeAssignableTo<ConstantExpression>();
            chainedExpressionsCount.Should().Be(4);
        }

        [Fact]
        public void AndAlso_GivenCollectionOfExpressionsWithNoneExpressions_ThrowsArgumentException()
        {
            // Arrange
            var expressions = Array.Empty<Expression>();

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.AndAlso(expressions));

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("expressions");
        }

        [Fact]
        public void AndAlso_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Constant(true);
            var rightExpression = Expression.Constant(false);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.AndAlso(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.AndAlso);

            var actualAsBinaryExpression = actual as BinaryExpression;
            actualAsBinaryExpression.Left.Should().Be(leftExpression);
            actualAsBinaryExpression.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void AndAlso_GivenNullCollectionOfExpressions_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.AndAlso(null));

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("expressions");
        }

        [Fact]
        public void Assign_GivenLeftAndRightExpressions_AddsExpressionToExpressionsList()
        {
            // Arrange
            var leftExpression = Expression.Variable(type: typeof(int), name: "testVariable");
            var rightExpression = Expression.Constant(1);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.Assign(leftExpression, rightExpression);

            // Assert
            expressionBlockBuilder.Expressions.Should().HaveCount(1);
            var expression = expressionBlockBuilder.Expressions[0];
            expression.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            expression.NodeType.Should().Be(ExpressionType.Assign);
            var binaryExpression = expression as BinaryExpression;
            binaryExpression.Left.Should().NotBeNull()
                .And.BeSameAs(leftExpression);
            binaryExpression.Right.Should().NotBeNull()
                .And.BeSameAs(rightExpression);
        }

        [Fact]
        public void Block_GivenNotNullOrEmptyScopeAndNotNullImplementationBuilderWithLogicToAddExpressions_ReturnsExpression()
        {
            // Arrange
            var scopeName = "testScope";

            var expressionConfiguration = new ExpressionConfiguration();
            var childExpressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(childExpressionBlockBuilder)
                .SetupGet(x => x.Expressions)
                .Returns(() =>
                {
                    var valueToReturnExpression = Expression.Constant(1);
                    var labelTarget = Expression.Label("label");
                    var returnExpression = Expression.Return(labelTarget, valueToReturnExpression);

                    return new Expression[] { returnExpression };
                });
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionBlockBuilder(It.IsAny<string>(), It.IsAny<IExpressionBlockBuilder>(), It.IsAny<ExpressionConfiguration>()))
                .Returns(childExpressionBlockBuilder);
            IExpressionBlockBuilder actualChildExpressionBlockBuilder = null;

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Block(scopeName, b =>
            {
                actualChildExpressionBlockBuilder = b;
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BlockExpression>();
            actualChildExpressionBlockBuilder.Should().NotBeNull()
                .And.BeSameAs(childExpressionBlockBuilder);
        }

        [Fact]
        public void Block_GivenNotNullOrEmptyScopeAndNotNullImplementationBuilderWithoutLogic_ThrowsInvalidOperationException()
        {
            // Arrange
            var scopeName = "testScope";

            var expressionConfiguration = new ExpressionConfiguration();
            var childExpressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(childExpressionBlockBuilder)
                .SetupGet(x => x.Expressions)
                .Returns(Array.Empty<Expression>());
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionBlockBuilder(It.IsAny<string>(), It.IsAny<IExpressionBlockBuilder>(), It.IsAny<ExpressionConfiguration>()))
                .Returns(childExpressionBlockBuilder);
            IExpressionBlockBuilder actualChildExpressionBlockBuilder = null;

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.Block(scopeName, b =>
            {
                actualChildExpressionBlockBuilder = b;
            }));

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Block_GivenNotNullOrEmptyScopeAndNullImplementationBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var scopeName = "testScope";

            var expressionConfiguration = new ExpressionConfiguration();
            var childExpressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionBlockBuilder(It.IsAny<string>(), It.IsAny<IExpressionBlockBuilder>(), It.IsAny<ExpressionConfiguration>()))
                .Returns(childExpressionBlockBuilder);

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.Block(scopeName, null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("implementationBuilder");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Block_GivenNullOrEmptyScopeAndNotNullImplementationBuilderWithLogicToAddExpressions_ReturnsExpression(string scopeName)
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var childExpressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(childExpressionBlockBuilder)
                .SetupGet(x => x.Expressions)
                .Returns(() =>
                {
                    var valueToReturnExpression = Expression.Constant(1);
                    var labelTarget = Expression.Label("label");
                    var returnExpression = Expression.Return(labelTarget, valueToReturnExpression);

                    return new Expression[] { returnExpression };
                });
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionBlockBuilder(It.IsAny<string>(), It.IsAny<IExpressionBlockBuilder>(), It.IsAny<ExpressionConfiguration>()))
                .Returns(childExpressionBlockBuilder);
            IExpressionBlockBuilder actualChildExpressionBlockBuilder = null;

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Block(scopeName, b =>
            {
                actualChildExpressionBlockBuilder = b;
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BlockExpression>();
            actualChildExpressionBlockBuilder.Should().NotBeNull()
                .And.BeSameAs(childExpressionBlockBuilder);
        }

        [Fact]
        public void Call_GivenInstanceExpressionMethodInfo_ReturnsExpression()
        {
            // Arrange
            var instanceExpression = Expression.Variable(typeof(int));
            var methodInfo = typeof(int).GetMethod(nameof(ToString), Array.Empty<Type>());

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Call(instanceExpression, methodInfo);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<MethodCallExpression>();
        }

        [Fact]
        public void Call_GivenInstanceExpressionMethodInfoAndParameters_ReturnsExpression()
        {
            // Arrange
            var instanceExpression = Expression.Variable(typeof(int));
            var methodInfo = typeof(int).GetMethod(nameof(ToString), new[] { typeof(string) });
            var parameterExpressions = new Expression[] { Expression.Constant("format") };

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Call(instanceExpression, methodInfo, parameterExpressions);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<MethodCallExpression>();
        }

        [Fact]
        public void Constant_GivenValueAndType_ReturnsExpression()
        {
            // Arrange
            var constantValue = true;
            var constantType = typeof(bool);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Constant(constantValue, constantType);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<ConstantExpression>();
        }

        [Fact]
        public void Constant_GivenValueWithGenericType_ReturnsExpression()
        {
            // Arrange
            var constantValue = true;

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Constant(constantValue);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<ConstantExpression>();
        }

        [Fact]
        public void ConvertChecked_GivenExpressionAndTypeAsGeneric_ReturnsExpression()
        {
            // Arrange
            var expressionToConvert = Expression.Constant("test", typeof(object));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.ConvertChecked<string>(expressionToConvert);

            // Assert
            actual.Should().NotBeNull();
            actual.NodeType.Should().Be(ExpressionType.Convert);
        }

        [Fact]
        public void ConvertChecked_GivenExpressionAndTypeAsParameter_ReturnsExpression()
        {
            // Arrange
            var expressionToConvert = Expression.Constant("test", typeof(object));
            var typeToConvert = typeof(string);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.ConvertChecked(expressionToConvert, typeToConvert);

            // Assert
            actual.Should().NotBeNull();
            actual.NodeType.Should().Be(ExpressionType.Convert);
        }

        [Fact]
        public void CreateLabelTarget_GivenNameWithoutParentAndLabelNameAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var labelTargetName = "testLabel";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);
            expressionBlockBuilder.CreateLabelTarget(labelTargetName);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.CreateLabelTarget(labelTargetName));

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateLabelTarget_GivenNameWithoutParentAndLabelNameDoesNotExist_AddsNewLabelTargetAndReturns()
        {
            // Arrange
            var labelTargetName = "testLabel";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.CreateLabelTarget(labelTargetName);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(labelTargetName);
            expressionBlockBuilder.LabelTargets.Should().HaveCount(1)
                .And.ContainKey(labelTargetName)
                .And.ContainValue(actual);
        }

        [Fact]
        public void CreateLabelTarget_GivenNameWithParentAndLabelNameAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var labelTargetName = "testLabel";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            var parent = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(parent)
                .Setup(x => x.CreateLabelTarget(labelTargetName))
                .Returns(Expression.Label(labelTargetName));
            var scopeName = "childScopeTest";

            var expressionBlockBuilder = new ExpressionBlockBuilder(scopeName, parent, expressionBuilderFactory, expressionConfiguration);
            expressionBlockBuilder.CreateLabelTarget(labelTargetName);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.CreateLabelTarget(labelTargetName));

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>()
                .Which.Message.Should().Contain(scopeName);
        }

        [Fact]
        public void CreateLabelTarget_GivenNameWithParentAndLabelNameDoesNotExist_AddsNewLabelTargetAndReturns()
        {
            // Arrange
            var labelTargetName = "testLabel";
            var scopeName = "childScopeTest";
            var parentLabelTargetName = $"{scopeName}_{labelTargetName}";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            var parent = Mock.Of<IExpressionBlockBuilder>();
            string actualParentLabelTargetName = null;
            Mock.Get(parent)
                .Setup(x => x.CreateLabelTarget(It.IsAny<string>()))
                .Returns<string>(name =>
                {
                    actualParentLabelTargetName = name;
                    return Expression.Label(name);
                });

            var expressionBlockBuilder = new ExpressionBlockBuilder(scopeName, parent, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.CreateLabelTarget(labelTargetName);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(parentLabelTargetName);
            expressionBlockBuilder.LabelTargets.Should().HaveCount(1)
                .And.ContainKey(labelTargetName)
                .And.ContainValue(actual);
            actualParentLabelTargetName.Should().Be(parentLabelTargetName);
        }

        [Fact]
        public void CreateVariable_GivenNameAndTypeAsGenericWithoutParentAndNameDoesNotExist_AddsNewVariableAndReturns()
        {
            // Arrange
            var variableName = "testVariable";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.CreateVariable<int>(variableName);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<ParameterExpression>();
            actual.Name.Should().Be(variableName);
            actual.Type.Should().Be(typeof(int));
            expressionBlockBuilder.Variables.Should().HaveCount(1)
                .And.ContainKey(variableName)
                .And.ContainValue(actual);
        }

        [Fact]
        public void CreateVariable_GivenNameAndTypeWithoutParentAndNameAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var variableName = "testVariable";
            var variableType = typeof(int);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);
            expressionBlockBuilder.CreateVariable(variableName, variableType);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.CreateVariable(variableName, variableType));

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateVariable_GivenNameAndTypeWithoutParentAndNameDoesNotExist_AddsNewVariableAndReturns()
        {
            // Arrange
            var variableName = "testVariable";
            var variableType = typeof(int);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.CreateVariable(variableName, variableType);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<ParameterExpression>();
            actual.Name.Should().Be(variableName);
            actual.Type.Should().Be(variableType);
            expressionBlockBuilder.Variables.Should().HaveCount(1)
                .And.ContainKey(variableName)
                .And.ContainValue(actual);
        }

        [Fact]
        public void CreateVariable_GivenNameAndTypeWithParentAndNameAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var variableName = "testVariable";
            var variableType = typeof(int);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            var parent = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(parent)
                .Setup(x => x.CreateVariable(variableName, variableType))
                .Returns(Expression.Variable(variableType, variableName));
            var scopeName = "childScopeTest";

            var expressionBlockBuilder = new ExpressionBlockBuilder(scopeName, parent, expressionBuilderFactory, expressionConfiguration);
            expressionBlockBuilder.CreateVariable(variableName, variableType);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.CreateVariable(variableName, variableType));

            // Assert
            action.Should().ThrowExactly<InvalidOperationException>()
                .Which.Message.Should().Contain(scopeName);
        }

        [Fact]
        public void CreateVariable_GivenNameAndTypeWithParentAndNameDoesNotExist_AddsNewVariableAndReturns()
        {
            // Arrange
            var variableName = "testVariable";
            var variableType = typeof(int);
            var scopeName = "childScopeTest";
            var parentLabelTargetName = $"{scopeName}_{variableName}";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            var parent = Mock.Of<IExpressionBlockBuilder>();
            string actualParentLabelTargetName = null;
            Mock.Get(parent)
                .Setup(x => x.CreateVariable(It.IsAny<string>(), It.IsAny<Type>()))
                .Returns<string, Type>((name, type) =>
                {
                    actualParentLabelTargetName = name;
                    return Expression.Variable(type, name);
                });

            var expressionBlockBuilder = new ExpressionBlockBuilder(scopeName, parent, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.CreateVariable(variableName, variableType);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<ParameterExpression>();
            actual.Name.Should().Be(parentLabelTargetName);
            actual.Type.Should().Be(variableType);
            expressionBlockBuilder.Variables.Should().HaveCount(1)
                .And.ContainKey(variableName)
                .And.ContainValue(actual);
            actualParentLabelTargetName.Should().Be(parentLabelTargetName);
        }

        [Fact]
        public void Empty_NoConditions_ReturnsExpression()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Empty();

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<DefaultExpression>();
        }

        [Fact]
        public void Equal_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Parameter(typeof(int), "x");
            var rightExpression = Expression.Constant(0, typeof(int));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Equal(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.Equal);
            var actualBinary = actual as BinaryExpression;
            actualBinary.Left.Should().Be(leftExpression);
            actualBinary.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void GetLabelTarget_GivenNameForExistentLabelTarget_ReturnsLabelTarget()
        {
            // Arrange
            var labelTargetName = "testLabel";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);
            var expected = expressionBlockBuilder.CreateLabelTarget(labelTargetName);

            // Act
            var actual = expressionBlockBuilder.GetLabelTarget(labelTargetName);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expected);
            actual.Name.Should().Be(labelTargetName);
        }

        [Fact]
        public void GetLabelTarget_GivenNameForNonExistentLabelTarget_ThrowsKeyNotFoundException()
        {
            // Arrange
            var labelTargetName = "testLabel";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.GetLabelTarget(labelTargetName));

            // Assert
            action.Should().ThrowExactly<KeyNotFoundException>()
                .Which.Message.Should().Contain(labelTargetName);
        }

        [Fact]
        public void GetParameter_GivenNameForExistentParameter_ReturnsParameterExpression()
        {
            // Arrange
            var parameterName = "testParameter";
            var expected = Expression.Parameter(typeof(int), parameterName);

            var expressionConfiguration = new ExpressionConfiguration
            {
                Parameters = new Dictionary<string, ParameterExpression>
                {
                    { parameterName, expected }
                }
            };
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.GetParameter(parameterName);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expected);
            actual.Name.Should().Be(parameterName);
        }

        [Fact]
        public void GetParameter_GivenNameForNonExistentParameter_ThrowsKeyNotFoundException()
        {
            // Arrange
            var parameterName = "testParameter";

            var expressionConfiguration = new ExpressionConfiguration
            {
                Parameters = new Dictionary<string, ParameterExpression>()
            };
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.GetParameter(parameterName));

            // Assert
            action.Should().ThrowExactly<KeyNotFoundException>()
                .Which.Message.Should().Contain(parameterName);
        }

        [Fact]
        public void GetVariable_GivenNameForExistentVariable_ReturnsParameterExpression()
        {
            // Arrange
            var variableName = "testVariable";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);
            var expected = expressionBlockBuilder.CreateVariable<int>(variableName);

            // Act
            var actual = expressionBlockBuilder.GetVariable(variableName);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expected);
            actual.Name.Should().Be(variableName);
        }

        [Fact]
        public void GetVariable_GivenNameForNonExistentVariable_ThrowsKeyNotFoundException()
        {
            // Arrange
            var variableName = "testVariable";

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.GetVariable(variableName));

            // Assert
            action.Should().ThrowExactly<KeyNotFoundException>()
                .Which.Message.Should().Contain(variableName);
        }

        [Fact]
        public void Goto_GivenLabelTarget_CreatesGotoExpressionAndAddsToExpressionsList()
        {
            // Arrange
            var labelTarget = Expression.Label("testLabelTarget");

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.Goto(labelTarget);

            // Assert
            expressionBlockBuilder.Expressions.Should().HaveCount(1);
            var gotoExpression = expressionBlockBuilder.Expressions[0] as GotoExpression;
            gotoExpression.Should().NotBeNull();
            gotoExpression.Target.Should().BeSameAs(labelTarget);
        }

        [Fact]
        public void GreaterThan_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Parameter(typeof(int), "x");
            var rightExpression = Expression.Constant(0, typeof(int));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.GreaterThan(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.GreaterThan);
            var actualBinary = actual as BinaryExpression;
            actualBinary.Left.Should().Be(leftExpression);
            actualBinary.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void GreaterThanOrEqual_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Parameter(typeof(int), "x");
            var rightExpression = Expression.Constant(0, typeof(int));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.GreaterThanOrEqual(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.GreaterThanOrEqual);
            var actualBinary = actual as BinaryExpression;
            actualBinary.Left.Should().Be(leftExpression);
            actualBinary.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void If_GivenNullTestExpressionAndThenExpression_ThrowsArgumentNullException()
        {
            // Arrange
            var resultExpression = Expression.Variable(typeof(bool), "result");
            var thenExpression = Expression.Assign(resultExpression, Expression.Constant(true));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.If(null, b => thenExpression));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("evaluationExpressionBuilder");
        }

        [Fact]
        public void If_GivenTestExpressionAndNullThenExpression_ThrowsArgumentNullException()
        {
            // Arrange
            var testExpression = Expression.Equal(Expression.Variable(typeof(int), "x"), Expression.Constant(0, typeof(int)));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.If(b => testExpression, null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("thenExpressionBuilder");
        }

        [Fact]
        public void If_GivenTestExpressionAndThenExpression_CreatesConditionalExpressionAndAddsToExpressionsList()
        {
            // Arrange
            var resultExpression = Expression.Variable(typeof(bool), "result");
            var testExpression = Expression.Equal(Expression.Variable(typeof(int), "x"), Expression.Constant(0, typeof(int)));
            var thenExpression = Expression.Assign(resultExpression, Expression.Constant(true));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.If(b => testExpression, b => thenExpression);

            // Assert
            expressionBlockBuilder.Expressions.Count.Should().Be(1);
            var expression = expressionBlockBuilder.Expressions[0];
            expression.Should().NotBeNull()
                .And.BeAssignableTo<ConditionalExpression>();
            expression.NodeType.Should().Be(ExpressionType.Conditional);
            var conditionalExpression = expression as ConditionalExpression;
            conditionalExpression.Test.Should().Be(testExpression);
            conditionalExpression.IfTrue.Should().Be(thenExpression);
            conditionalExpression.IfFalse.Should().NotBeNull();
            conditionalExpression.IfFalse.NodeType.Should().Be(ExpressionType.Default);
        }

        [Fact]
        public void If_GivenTestExpressionThenExpressionAndElseExpression_CreatesConditionalExpressionAndAddsToExpressionsList()
        {
            // Arrange
            var resultExpression = Expression.Variable(typeof(bool), "result");
            var testExpression = Expression.Equal(Expression.Variable(typeof(int), "x"), Expression.Constant(0, typeof(int)));
            var thenExpression = Expression.Assign(resultExpression, Expression.Constant(true));
            var elseExpression = Expression.Assign(resultExpression, Expression.Constant(false));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.If(b => testExpression, b => thenExpression, b => elseExpression);

            // Assert
            expressionBlockBuilder.Expressions.Count.Should().Be(1);
            var expression = expressionBlockBuilder.Expressions[0];
            expression.Should().NotBeNull()
                .And.BeAssignableTo<ConditionalExpression>();
            expression.NodeType.Should().Be(ExpressionType.Conditional);
            var conditionalExpression = expression as ConditionalExpression;
            conditionalExpression.Test.Should().Be(testExpression);
            conditionalExpression.IfTrue.Should().Be(thenExpression);
            conditionalExpression.IfFalse.Should().Be(elseExpression);
        }

        [Fact]
        public void Label_GivenLabelTarget_CreatesLabelExpressionAndAddsToExpressionsList()
        {
            // Arrange
            var labelTarget = Expression.Label("testLabel");

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.Label(labelTarget);

            // Assert
            expressionBlockBuilder.Expressions.Should().HaveCount(1);
            var expression = expressionBlockBuilder.Expressions[0];
            expression.Should().NotBeNull()
                .And.BeAssignableTo<LabelExpression>();
            var labelExpression = expression as LabelExpression;
            labelExpression.Target.Should().Be(labelTarget);
        }

        [Fact]
        public void LessThan_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Parameter(typeof(int), "x");
            var rightExpression = Expression.Constant(0, typeof(int));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.LessThan(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.LessThan);
            var actualBinary = actual as BinaryExpression;
            actualBinary.Left.Should().Be(leftExpression);
            actualBinary.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void LessThanOrEqual_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Parameter(typeof(int), "x");
            var rightExpression = Expression.Constant(0, typeof(int));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.LessThanOrEqual(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.LessThanOrEqual);
            var actualBinary = actual as BinaryExpression;
            actualBinary.Left.Should().Be(leftExpression);
            actualBinary.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void Not_GivenExpression_ReturnsExpression()
        {
            // Arrange
            var originalExpression = Expression.Constant(false, typeof(bool));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.Not(originalExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<UnaryExpression>();
            var unaryExpression = actual as UnaryExpression;
            unaryExpression.Operand.Should().Be(originalExpression);
        }

        [Fact]
        public void NotEqual_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Parameter(typeof(int), "x");
            var rightExpression = Expression.Constant(0, typeof(int));

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.NotEqual(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.NotEqual);
            var actualBinary = actual as BinaryExpression;
            actualBinary.Left.Should().Be(leftExpression);
            actualBinary.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void OrElse_GivenCollectionOfExpressionsWithMultipleExpressions_ReturnsExpression()
        {
            // Arrange
            var expressions = new Expression[]
            {
                Expression.Constant(true),
                Expression.Constant(true),
                Expression.Constant(true),
                Expression.Constant(true),
                Expression.Constant(true),
            };

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.OrElse(expressions);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.OrElse);

            var actualAsBinaryExpression = actual as BinaryExpression;
            actualAsBinaryExpression.Right.Should().BeAssignableTo<ConstantExpression>();
            actualAsBinaryExpression.Left.Should().BeAssignableTo<BinaryExpression>();
            var chainedExpressionsCount = 1;
            while (actualAsBinaryExpression.Left is BinaryExpression)
            {
                actualAsBinaryExpression.Right.Should().BeAssignableTo<ConstantExpression>();
                actualAsBinaryExpression = actualAsBinaryExpression.Left as BinaryExpression;
                chainedExpressionsCount++;
            }

            actualAsBinaryExpression.Left.Should().BeAssignableTo<ConstantExpression>();
            actualAsBinaryExpression.Right.Should().BeAssignableTo<ConstantExpression>();
            chainedExpressionsCount.Should().Be(4);
        }

        [Fact]
        public void OrElse_GivenCollectionOfExpressionsWithNoneExpressions_ThrowsArgumentException()
        {
            // Arrange
            var expressions = Array.Empty<Expression>();

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.OrElse(expressions));

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("expressions");
        }

        [Fact]
        public void OrElse_GivenLeftAndRightExpressions_ReturnsExpression()
        {
            // Arrange
            var leftExpression = Expression.Constant(true);
            var rightExpression = Expression.Constant(false);

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var actual = expressionBlockBuilder.OrElse(leftExpression, rightExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeAssignableTo<BinaryExpression>();
            actual.NodeType.Should().Be(ExpressionType.OrElse);

            var actualAsBinaryExpression = actual as BinaryExpression;
            actualAsBinaryExpression.Left.Should().Be(leftExpression);
            actualAsBinaryExpression.Right.Should().Be(rightExpression);
        }

        [Fact]
        public void OrElse_GivenNullCollectionOfExpressions_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.OrElse(null));

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("expressions");
        }

        [Fact]
        public void Return_GivenNullReturnValueExpression_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration
            {
                ReturnLabelTarget = Expression.Label(typeof(string)),
            };
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.Return(null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("returnValueExpression");
        }

        [Fact]
        public void Return_GivenReturnValueExpression_CreatesReturnExpressionAndAddsToExpressionsList()
        {
            // Arrange
            var returnValueExpression = Expression.Constant("testValue");

            var expressionConfiguration = new ExpressionConfiguration
            {
                ReturnLabelTarget = Expression.Label(typeof(string)),
            };
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.Return(returnValueExpression);

            // Assert
            expressionBlockBuilder.Expressions.Should().HaveCount(1);
            var returnExpression = expressionBlockBuilder.Expressions[0];
            returnExpression.Should().NotBeNull()
                .And.BeAssignableTo<GotoExpression>();
            var gotoExpression = returnExpression as GotoExpression;
            gotoExpression.Target.Should().NotBeNull()
                .And.BeSameAs(expressionConfiguration.ReturnLabelTarget);
            gotoExpression.Value.Should().Be(returnValueExpression);
        }

        [Fact]
        public void Switch_GivenNullSwitchValueExpressionAndSwitchExpressionBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.Switch(null, b => { }));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("switchValueExpression");
        }

        [Fact]
        public void Switch_GivenSwitchValueExpressionAndNullSwitchExpressionBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var switchValueExpression = Expression.Variable(typeof(string), "input");

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            var action = FluentActions.Invoking(() => expressionBlockBuilder.Switch(switchValueExpression, null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("expressionSwitchBuilderAction");
        }

        [Fact]
        public void Switch_GivenSwitchValueExpressionAndSwitchExpressionBuilder_CreatesSwitchExpressionAndAddsToExpressionsList()
        {
            // Arrange
            var switchValueExpression = Expression.Variable(typeof(string), "input");
            var resultVariable = Expression.Variable(typeof(bool), "result");
            var expectedDefaultBody = Expression.Assign(resultVariable, Expression.Constant(false));
            var expectedSwitchCases = new[]
            {
                Expression.SwitchCase(Expression.Assign(resultVariable, Expression.Constant(true)), Expression.Constant("A")),
                Expression.SwitchCase(Expression.Assign(resultVariable, Expression.Constant(false)), Expression.Constant("B")),
                Expression.SwitchCase(Expression.Assign(resultVariable, Expression.Constant(true)), Expression.Constant("C")),
            };

            var expressionConfiguration = new ExpressionConfiguration();
            var expressionSwitchBuilder = Mock.Of<IExpressionSwitchBuilder>();
            Mock.Get(expressionSwitchBuilder)
                .SetupGet(x => x.DefaultBody)
                .Returns(expectedDefaultBody);
            Mock.Get(expressionSwitchBuilder)
                .SetupGet(x => x.SwitchCases)
                .Returns(expectedSwitchCases);
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionSwitchBuilder(It.IsAny<IExpressionBlockBuilder>()))
                .Returns(expressionSwitchBuilder);

            var expressionBlockBuilder = new ExpressionBlockBuilder(string.Empty, null, expressionBuilderFactory, expressionConfiguration);

            // Act
            expressionBlockBuilder.Switch(switchValueExpression, b => { });

            // Assert
            expressionBlockBuilder.Expressions.Should().HaveCount(1);
            var expression = expressionBlockBuilder.Expressions[0];
            expression.Should().NotBeNull()
                .And.BeAssignableTo<SwitchExpression>();
            var switchExpression = expression as SwitchExpression;
            switchExpression.Should().NotBeNull();
            switchExpression.SwitchValue.Should().BeSameAs(switchValueExpression);
            switchExpression.Cases.Should().BeEquivalentTo(expectedSwitchCases);
        }
    }
}