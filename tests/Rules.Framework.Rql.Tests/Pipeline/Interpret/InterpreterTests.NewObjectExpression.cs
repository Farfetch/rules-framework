namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Collections.Generic;
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
        public async Task VisitNewObjectExpression_GivenValidNewObjectExpressionWithPropertiesInitializer_ReturnsObjectWithPropertiesFilled()
        {
            // Arrange
            var objectToken = NewToken("object", null, Framework.Rql.Tokens.TokenType.OBJECT);
            var assignementToken = NewToken("=", null, Framework.Rql.Tokens.TokenType.ASSIGN);
            var values = new[]
            {
                new AssignmentExpression(
                    CreateMockedExpression(NewRqlString("Name")),
                    assignementToken,
                    CreateMockedExpression(NewRqlString("Roger"))),
                new AssignmentExpression(
                    CreateMockedExpression(NewRqlString("Age")),
                    assignementToken,
                    CreateMockedExpression(NewRqlInteger(25))),
            };

            var newObjectExpression = new NewObjectExpression(objectToken, values);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNewObjectExpression(newObjectExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<RqlObject>();
            var objProperties = (IDictionary<string, object>)actual.RuntimeValue;
            objProperties.Should().NotBeNullOrEmpty()
                .And.Contain("Name", "Roger")
                .And.Contain("Age", 25);
        }
    }
}