namespace Rules.Framework.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ExpressionSwitchBuilderTests
    {
        [Fact]
        public void Case_GivenCaseExpressionAndCaseBodyExpressionBuilder_CreatesSwitchCaseAndAddsToSwitchCasesList()
        {
            // Arrange
            var expectedCaseValueExpression = Expression.Constant("A");
            var resultVariable = Expression.Variable(typeof(int), "result");
            var expectedCaseBodyExpression = Expression.Assign(resultVariable, Expression.Constant(1));
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            var actual = expressionSwitchBuilder.Case(expectedCaseValueExpression, b => expectedCaseBodyExpression);

            // Assert
            actual.Should().BeSameAs(expressionSwitchBuilder);
            expressionSwitchBuilder.SwitchCases.Should().HaveCount(1);
            var actualSwitchCase = expressionSwitchBuilder.SwitchCases.First();
            actualSwitchCase.Body.Should().Be(expectedCaseBodyExpression);
            actualSwitchCase.TestValues.Should().HaveCount(1)
                .And.Contain(expectedCaseValueExpression);
        }

        [Fact]
        public void Case_GivenCaseExpressionAndNullCaseBodyExpressionBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var expectedCaseValueExpression = Expression.Constant("A");
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            var action = FluentActions.Invoking(() => expressionSwitchBuilder.Case(expectedCaseValueExpression, null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("caseBodyExpressionBuilder");
        }

        [Fact]
        public void Case_GivenCaseExpressionsAndCaseBodyExpressionBuilder_CreatesSwitchCaseAndAddsToSwitchCasesList()
        {
            // Arrange
            var expectedCaseValueExpressions = new Expression[]
            {
                Expression.Constant("A"),
                Expression.Constant("B"),
            };
            var resultVariable = Expression.Variable(typeof(int), "result");
            var expectedCaseBodyExpression = Expression.Assign(resultVariable, Expression.Constant(1));
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            var actual = expressionSwitchBuilder.Case(expectedCaseValueExpressions, b => expectedCaseBodyExpression);

            // Assert
            actual.Should().BeSameAs(expressionSwitchBuilder);
            expressionSwitchBuilder.SwitchCases.Should().HaveCount(1);
            var actualSwitchCase = expressionSwitchBuilder.SwitchCases.First();
            actualSwitchCase.Body.Should().Be(expectedCaseBodyExpression);
            actualSwitchCase.TestValues.Should().HaveCount(2)
                .And.Contain(expectedCaseValueExpressions);
        }

        [Fact]
        public void Case_GivenNullCaseExpressionAndCaseBodyExpressionBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var resultVariable = Expression.Variable(typeof(int), "result");
            var expectedCaseBodyExpression = Expression.Assign(resultVariable, Expression.Constant(1));
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            var action = FluentActions.Invoking(() => expressionSwitchBuilder.Case(caseExpression: null, b => expectedCaseBodyExpression));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("caseExpression");
        }

        [Fact]
        public void Case_GivenNullCaseExpressionsAndCaseBodyExpressionBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var resultVariable = Expression.Variable(typeof(int), "result");
            var expectedCaseBodyExpression = Expression.Assign(resultVariable, Expression.Constant(1));
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            var action = FluentActions.Invoking(() => expressionSwitchBuilder.Case(caseExpressions: null, b => expectedCaseBodyExpression));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("caseExpressions");
        }

        [Fact]
        public void Default_GivenCaseBodyExpressionBuilder_SetsDefaultBody()
        {
            // Arrange
            var resultVariable = Expression.Variable(typeof(int), "result");
            var expectedDefaultBodyExpression = Expression.Assign(resultVariable, Expression.Constant(1));
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            expressionSwitchBuilder.Default(b => expectedDefaultBodyExpression);

            // Assert
            expressionSwitchBuilder.DefaultBody.Should().NotBeNull()
                .And.BeSameAs(expectedDefaultBodyExpression);
        }

        [Fact]
        public void Default_GivenNullCaseBodyExpressionBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionBlockBuilder = Mock.Of<IExpressionBlockBuilder>();
            var expressionSwitchBuilder = new ExpressionSwitchBuilder(expressionBlockBuilder);

            // Act
            var action = FluentActions.Invoking(() => expressionSwitchBuilder.Default(null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("defaultBodyExpressionBuilder");
        }
    }
}