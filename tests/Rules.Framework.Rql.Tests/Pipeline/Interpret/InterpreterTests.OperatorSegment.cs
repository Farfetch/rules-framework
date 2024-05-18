namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tokens;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Theory]
        [InlineData(new object[] { TokenType.AND }, RqlOperators.And)]
        [InlineData(new object[] { TokenType.ASSIGN }, RqlOperators.Assign)]
        [InlineData(new object[] { TokenType.EQUAL }, RqlOperators.Equals)]
        [InlineData(new object[] { TokenType.GREATER_THAN }, RqlOperators.GreaterThan)]
        [InlineData(new object[] { TokenType.GREATER_THAN_OR_EQUAL }, RqlOperators.GreaterThanOrEquals)]
        [InlineData(new object[] { TokenType.IN }, RqlOperators.In)]
        [InlineData(new object[] { TokenType.LESS_THAN }, RqlOperators.LesserThan)]
        [InlineData(new object[] { TokenType.LESS_THAN_OR_EQUAL }, RqlOperators.LesserThanOrEquals)]
        [InlineData(new object[] { TokenType.MINUS }, RqlOperators.Minus)]
        [InlineData(new object[] { TokenType.NOT, TokenType.IN }, RqlOperators.NotIn)]
        [InlineData(new object[] { TokenType.NOT_EQUAL }, RqlOperators.NotEquals)]
        [InlineData(new object[] { TokenType.OR }, RqlOperators.Or)]
        [InlineData(new object[] { TokenType.PLUS }, RqlOperators.Plus)]
        [InlineData(new object[] { TokenType.SLASH }, RqlOperators.Slash)]
        [InlineData(new object[] { TokenType.STAR }, RqlOperators.Star)]
        public async Task VisitOperatorSegment_GivenOperatorSegmentWithSupportedOperatorToken_ReturnsRqlOperator(object[] tokenTypes, object expected)
        {
            // Arrange
            var operatorSegment = new OperatorSegment(tokenTypes.Select(tt => NewToken("test", null, (TokenType)tt)).ToArray());

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitOperatorSegment(operatorSegment);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task VisitOperatorSegment_GivenOperatorSegmentWithUnsupportedOperatorToken_ThrowsNotSupportedException()
        {
            // Arrange
            var operatorSegment = new OperatorSegment(new[] { NewToken("test", null, TokenType.NOT) });

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actualException = await Assert.ThrowsAsync<NotSupportedException>(async () => await interpreter.VisitOperatorSegment(operatorSegment));

            // Assert
            actualException.Message.Should().Be($"The tokens with types ['NOT'] are not supported as a valid operator.");
        }

        [Theory]
        [InlineData(TokenType.ALL, TokenType.INT)]
        [InlineData(TokenType.NOT, TokenType.INT)]
        public async Task VisitOperatorSegment_GivenOperatorSegmentWithUnsupportedOperatorTokens_ThrowsNotSupportedException(object tokenType1, object tokenType2)
        {
            // Arrange
            var operatorSegment = new OperatorSegment(new[] { NewToken("test", null, (TokenType)tokenType1), NewToken("test", null, (TokenType)tokenType2) });

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actualException = await Assert.ThrowsAsync<NotSupportedException>(async () => await interpreter.VisitOperatorSegment(operatorSegment));

            // Assert
            actualException.Message.Should().Be($"The tokens with types ['{tokenType1}', '{tokenType2}'] are not supported as a valid operator.");
        }
    }
}