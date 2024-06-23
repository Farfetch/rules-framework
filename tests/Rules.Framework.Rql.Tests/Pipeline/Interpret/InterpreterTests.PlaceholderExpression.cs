namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
        public async Task VisitPlaceholderExpression_GivenPlaceholderExpression_ReturnsRqlStringWithPlaceholderName()
        {
            // Arrange
            var placeholderExpression = new PlaceholderExpression(NewToken("testPlaceholder", "testPlaceholder", Framework.Rql.Tokens.TokenType.PLACEHOLDER));

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitPlaceholderExpression(placeholderExpression);

            // Assert
            actual.Should().BeOfType<RqlString>();
            var actualString = (RqlString)actual;
            actualString.Value.Should().Be("testPlaceholder");
        }
    }
}