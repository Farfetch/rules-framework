namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitBinaryExpression_GivenValidBinaryExpression_ProcessesRuleBinary()
        {
            // Arrange
            var expected = NewRqlBool(false);
            var leftExpression = CreateMockedExpression(NewRqlString("message"));
            var operatorSegment = CreateMockedSegment(RqlOperators.Equals);
            var rightExpression = CreateMockedExpression(NewRqlString("Hello world"));
            var binaryExpression = new BinaryExpression(leftExpression, operatorSegment, rightExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(r => r.ApplyBinary(It.IsAny<IRuntimeValue>(), It.IsAny<RqlOperators>(), It.IsAny<IRuntimeValue>()))
                .Returns(expected);
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitBinaryExpression(binaryExpression);

            // Assert
            actual.Should().Be(expected);
            Mock.Get(runtime)
                .Verify(r => r.ApplyBinary(It.IsAny<IRuntimeValue>(), It.IsAny<RqlOperators>(), It.IsAny<IRuntimeValue>()), Times.Once());
        }

        [Fact]
        public async Task VisitBinaryExpression_GivenValidBinaryExpressionFailingBinaryOnRuntime_ThrowsInterpreterExceptionWithErrorMessageFromRuntime()
        {
            // Arrange
            var leftExpression = CreateMockedExpression(NewRqlString("message"));
            var operatorSegment = CreateMockedSegment(RqlOperators.Equals);
            var rightExpression = CreateMockedExpression(NewRqlString("Hello world"));
            var binaryExpression = new BinaryExpression(leftExpression, operatorSegment, rightExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            const string expected = "An error has occurred";
            Mock.Get(runtime)
                .Setup(r => r.ApplyBinary(It.IsAny<IRuntimeValue>(), It.IsAny<RqlOperators>(), It.IsAny<IRuntimeValue>()))
                .Throws(new RuntimeException(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var interpreterException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitBinaryExpression(binaryExpression));

            // Assert
            interpreterException.Should().NotBeNull();
            interpreterException.Message.Should().Contain(expected);
        }
    }
}