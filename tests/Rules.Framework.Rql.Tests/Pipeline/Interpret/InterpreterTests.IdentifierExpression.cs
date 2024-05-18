namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tokens;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitIdentifierExpression_GivenValidIdentifierExpression_ReturnsIdentifierLexeme()
        {
            // Arrange
            var expected = NewRqlString("test");
            var identifierToken = NewToken("test", null, TokenType.IDENTIFIER);
            var identifierExpression = new IdentifierExpression(identifierToken);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitIdentifierExpression(identifierExpression);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}