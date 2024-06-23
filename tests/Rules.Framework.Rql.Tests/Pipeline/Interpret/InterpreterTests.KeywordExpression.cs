namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tests.Stubs;
    using Rules.Framework.Rql.Tokens;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitKeywordExpression_GivenValidKeywordExpression_ReturnsLexeme()
        {
            // Arrange
            var expected = NewRqlString("var");
            var keywordToken = NewToken("var", null, TokenType.VAR);
            var keywordExpression = KeywordExpression.Create(keywordToken);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitKeywordExpression(keywordExpression);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}