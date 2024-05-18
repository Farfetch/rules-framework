namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System;
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
        public async Task VisitNewArrayExpression_GivenValidNewArrayExpressionWithSizeInitializer_ReturnsArrayFilledWithRqlNothing()
        {
            // Arrange
            var arrayToken = NewToken("array", null, Framework.Rql.Tokens.TokenType.ARRAY);
            var initializerBeginToken = NewToken("[", null, Framework.Rql.Tokens.TokenType.STRAIGHT_BRACKET_LEFT);
            var sizeExpression = CreateMockedExpression(NewRqlInteger(2));
            var values = Array.Empty<Expression>();
            var initializerEndToken = NewToken("]", null, Framework.Rql.Tokens.TokenType.STRAIGHT_BRACKET_RIGHT);

            var newArrayExpression = NewArrayExpression.Create(arrayToken, initializerBeginToken, sizeExpression, values, initializerEndToken);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNewArrayExpression(newArrayExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<RqlArray>();
            var array = (RqlArray)actual;
            array.Size.Should().Be(NewRqlInteger(2));
            array.Value.Should().AllSatisfy(i => i.Unwrap().Should().BeOfType<RqlNothing>());
        }

        [Fact]
        public async Task VisitNewArrayExpression_GivenValidNewArrayExpressionWithValuesInitializer_ReturnsArrayFilledWithValues()
        {
            // Arrange
            var arrayToken = NewToken("array", null, Framework.Rql.Tokens.TokenType.ARRAY);
            var initializerBeginToken = NewToken("{", null, Framework.Rql.Tokens.TokenType.BRACE_LEFT);
            var sizeExpression = CreateMockedExpression(NewRqlNothing());
            var values = new[]
            {
                CreateMockedExpression(NewRqlInteger(1)),
                CreateMockedExpression(NewRqlString("test")),
                CreateMockedExpression(NewRqlBool(true)),
            };
            var initializerEndToken = NewToken("}", null, Framework.Rql.Tokens.TokenType.BRACE_RIGHT);

            var newArrayExpression = NewArrayExpression.Create(arrayToken, initializerBeginToken, sizeExpression, values, initializerEndToken);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNewArrayExpression(newArrayExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<RqlArray>();
            var array = (RqlArray)actual;
            array.Size.Should().Be(NewRqlInteger(3));
            array.Value.Should().SatisfyRespectively(
                v => v.Unwrap<RqlInteger>().Value.Should().Be(1),
                v => v.Unwrap<RqlString>().Value.Should().Be("test"),
                v => v.Unwrap<RqlBool>().Value.Should().BeTrue());
        }
    }
}