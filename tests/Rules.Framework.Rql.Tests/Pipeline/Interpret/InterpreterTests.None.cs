namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitNoneExpression_GivenNoneExpression_ReturnsRqlNothing()
        {
            // Arrange
            var noneExpression = new NoneExpression();

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNoneExpression(noneExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<RqlNothing>();
        }

        [Fact]
        public async Task VisitNoneSegment_GivenNoneSegment_ReturnsNull()
        {
            // Arrange
            var noneSegment = new NoneSegment();

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNoneSegment(noneSegment);

            // Assert
            actual.Should().BeNull();
        }

        [Fact]
        public async Task VisitNoneStatement_GivenNoneStatement_ReturnsExpressionStatementWithRqlNothing()
        {
            // Arrange
            var noneStatement = new NoneStatement();

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNoneStatement(noneStatement);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionStatementResult>();
            var actualExpressionStatementResult = actual as ExpressionStatementResult;
            actualExpressionStatementResult.Rql.Should().BeEmpty();
            actualExpressionStatementResult.Result.Should().BeOfType<RqlNothing>();
        }
    }
}