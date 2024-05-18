namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitExpressionStatemet_GivenValidExpressionStatement_ReturnsExpressionResultWithRql()
        {
            // Arrange
            var expectedValue = NewRqlString("test");
            var expectedRql = "test rql";
            var expression = CreateMockedExpression(expectedValue);
            var expressionStatement = ExpressionStatement.Create(expression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();
            Mock.Get(reverseRqlBuilder)
                .Setup(x => x.BuildRql(It.IsIn(expressionStatement)))
                .Returns(expectedRql);

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitExpressionStatement(expressionStatement);

            // Assert
            actual.Should().NotBeNull().And.BeOfType<ExpressionStatementResult>();
            actual.Rql.Should().Be(expectedRql);
            actual.Success.Should().BeTrue();
            var actualExpressionStatementResult = actual as ExpressionStatementResult;
            actualExpressionStatementResult.Result.Should().BeEquivalentTo(expectedValue);
        }
    }
}