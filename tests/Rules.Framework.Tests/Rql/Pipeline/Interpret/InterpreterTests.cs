namespace Rules.Framework.Tests.Rql.Pipeline.Interpret
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
    using Rules.Framework.Rql.Tokens;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public void Dispose_NoParameters_TriggersDisposeOfRuntime()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            interpreter.Dispose();

            // Assert
            Mock.Get(runtime)
                .Verify(r => r.Dispose(), Times.Once());
        }

        [Fact]
        public async Task InterpretAsync_GivenInvalidStatementThatIssuesAnError_ReturnsErrorStatementResult()
        {
            // Arrange
            var expectedException = new InterpreterException("abc", "rql", RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 10));
            var expected = new ErrorStatementResult(expectedException.Message, expectedException.Rql, expectedException.BeginPosition, expectedException.EndPosition);
            var mockStatementToExecute = new Mock<Statement>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            mockStatementToExecute
                .Setup(s => s.Accept(It.IsAny<IStatementVisitor<Task<Framework.Rql.Pipeline.Interpret.IResult>>>()))
                .Throws(expectedException);
            var statements = new[] { mockStatementToExecute.Object };

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.InterpretAsync(statements);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<InterpretResult>();
            var interpretResult = actual as InterpretResult;
            interpretResult.Results.Should().HaveCount(1)
                .And.ContainEquivalentOf(expected);
        }

        [Fact]
        public async Task InterpretAsync_GivenValidStatement_ExecutesAndReturnsResult()
        {
            // Arrange
            var expected = new NothingStatementResult("abc");
            var mockStatementToExecute = new Mock<Statement>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            mockStatementToExecute
                .Setup(s => s.Accept(It.IsAny<IStatementVisitor<Task<Framework.Rql.Pipeline.Interpret.IResult>>>()))
                .Returns(Task.FromResult<Framework.Rql.Pipeline.Interpret.IResult>(expected));
            var statements = new[] { mockStatementToExecute.Object };

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.InterpretAsync(statements);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<InterpretResult>();
            var interpretResult = actual as InterpretResult;
            interpretResult.Results.Should().HaveCount(1)
                .And.Contain(expected);
        }

        private static Expression CreateMockedExpression(IRuntimeValue visitResult)
        {
            var mockExpression = new Mock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            mockExpression.Setup(e => e.Accept(It.IsAny<IExpressionVisitor<Task<IRuntimeValue>>>()))
                .Returns(Task.FromResult(visitResult));
            return mockExpression.Object;
        }

        private static Segment CreateMockedSegment(object visitResult)
        {
            var mockSegment = new Mock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            mockSegment.Setup(e => e.Accept(It.IsAny<ISegmentVisitor<Task<object>>>()))
                .Returns(Task.FromResult(visitResult));
            return mockSegment.Object;
        }

        private static Statement CreateMockedStatement(Framework.Rql.Pipeline.Interpret.IResult visitResult)
        {
            var mockStatement = new Mock<Statement>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            mockStatement.Setup(s => s.Accept(It.IsAny<IStatementVisitor<Task<Framework.Rql.Pipeline.Interpret.IResult>>>()))
                .Returns(Task.FromResult(visitResult));
            return mockStatement.Object;
        }

        private static RqlAny NewRqlAny(IRuntimeValue runtimeValue)
            => new RqlAny(runtimeValue);

        private static RqlArray NewRqlArray(params IRuntimeValue[] runtimeValues)
        {
            var rqlArray = new RqlArray(runtimeValues.Length);
            for (int i = 0; i < runtimeValues.Length; i++)
            {
                rqlArray.SetAtIndex(i, NewRqlAny(runtimeValues[i]));
            }

            return rqlArray;
        }

        private static RqlBool NewRqlBool(bool value)
            => new RqlBool(value);

        private static RqlDecimal NewRqlDecimal(decimal value)
            => new RqlDecimal(value);

        private static RqlInteger NewRqlInteger(int value)
                    => new RqlInteger(value);

        private static RqlString NewRqlString(string value)
            => new RqlString(value);

        private static Token NewToken(string lexeme, object value, TokenType type)
            => Token.Create(lexeme, false, value, RqlSourcePosition.Empty, RqlSourcePosition.Empty, (uint)lexeme.Length, type);
    }
}