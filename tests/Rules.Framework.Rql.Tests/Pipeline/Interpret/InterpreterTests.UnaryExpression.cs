namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitUnaryExpression_GivenUnaryExpressionWithKnownOperator_AppliesOperatorAndReturnsValue()
        {
            // Arrange
            var minusToken = NewToken("-", null, Framework.Rql.Tokens.TokenType.MINUS);
            var targetExpression = CreateMockedExpression(NewRqlInteger(10));
            var unaryExpression = new UnaryExpression(minusToken, targetExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(x => x.ApplyUnary(new RqlInteger(10), RqlOperators.Minus))
                .Returns(new RqlInteger(-10));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitUnaryExpression(unaryExpression);

            // Assert
            actual.Should().BeOfType<RqlInteger>()
                .And.Subject.As<RqlInteger>().Value.Should().Be(-10);
        }

        [Fact]
        public async Task VisitUnaryExpression_GivenUnaryExpressionWithUnknownOperator_ThrowsInterpreterException()
        {
            // Arrange
            var minusToken = NewToken("+", null, Framework.Rql.Tokens.TokenType.PLUS);
            var targetExpression = CreateMockedExpression(NewRqlInteger(10));
            var unaryExpression = new UnaryExpression(minusToken, targetExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(x => x.ApplyUnary(new RqlInteger(10), RqlOperators.None))
                .Throws(new RuntimeException("Unexpected operator"));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actualException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitUnaryExpression(unaryExpression));

            // Assert
            actualException.Message.Should().Contain("Unexpected operator");
        }
    }
}