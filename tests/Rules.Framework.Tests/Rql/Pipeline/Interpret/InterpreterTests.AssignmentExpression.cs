namespace Rules.Framework.Tests.Rql.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tokens;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitAssignmentExpression_GivenValidAssignmentExpression_ProcessesRuleAssignment()
        {
            // Arrange
            var expected = NewRqlArray();
            var variableNameExpression = CreateMockedExpression(NewRqlString("message"));
            var assignToken = NewToken("=", null, TokenType.ASSIGN);
            var valueExpression = CreateMockedExpression(NewRqlString("Hello world"));
            var assignmentExpression = new AssignmentExpression(variableNameExpression, assignToken, valueExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(r => r.Assign(It.IsAny<string>(), It.IsAny<IRuntimeValue>()))
                .Returns(new RqlNothing());
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitAssignmentExpression(assignmentExpression);

            // Assert
            actual.Should().Be(new RqlNothing());
            Mock.Get(runtime)
                .Verify(r => r.Assign(It.IsAny<string>(), It.IsAny<IRuntimeValue>()), Times.Once());
        }

        [Fact]
        public async Task VisitAssignmentExpression_GivenValidAssignmentExpressionFailingAssignmentOnRuntime_ThrowsInterpreterExceptionWithErrorMessageFromRuntime()
        {
            // Arrange
            var variableNameExpression = CreateMockedExpression(NewRqlString("message"));
            var assignToken = NewToken("=", null, TokenType.ASSIGN);
            var valueExpression = CreateMockedExpression(NewRqlAny(NewRqlString("Hello world")));
            var assignmentExpression = new AssignmentExpression(variableNameExpression, assignToken, valueExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            const string expected = "An error has occurred";
            Mock.Get(runtime)
                .Setup(r => r.Assign(It.IsAny<string>(), It.IsAny<IRuntimeValue>()))
                .Throws(new RuntimeException(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var interpreterException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitAssignmentExpression(assignmentExpression));

            // Assert
            interpreterException.Should().NotBeNull();
            interpreterException.Message.Should().Contain(expected);
        }
    }
}