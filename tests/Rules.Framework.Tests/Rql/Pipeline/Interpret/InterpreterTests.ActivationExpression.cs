namespace Rules.Framework.Tests.Rql.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitActivationExpression_GivenInvalidActivationExpressionWithNotExistentContentType_ThrowsInterpreterExceptionExpectingValidContentType()
        {
            // Arrange
            var expected = NewRqlArray();
            var ruleNameExpression = CreateMockedExpression(NewRqlAny(NewRqlString("Test rule name")));
            var contentTypeExpression = CreateMockedExpression(NewRqlAny(NewRqlString("NotDefinedType")));
            var activationExpression = new ActivationExpression(ruleNameExpression, contentTypeExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var interpreterException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitActivationExpression(activationExpression));

            // Assert
            interpreterException.Should().NotBeNull();
            interpreterException.Message.Should().Contain("The content type value 'NotDefinedType' was not found");
        }

        [Fact]
        public async Task VisitActivationExpression_GivenInvalidActivationExpressionWithRqlIntegerOnContentType_ThrowsInterpreterExceptionExpectingRqlString()
        {
            // Arrange
            var expected = NewRqlArray();
            var ruleNameExpression = CreateMockedExpression(NewRqlAny(NewRqlString("Test rule name")));
            var contentTypeExpression = CreateMockedExpression(NewRqlAny(NewRqlInteger(1)));
            var activationExpression = new ActivationExpression(ruleNameExpression, contentTypeExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var interpreterException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitActivationExpression(activationExpression));

            // Assert
            interpreterException.Should().NotBeNull();
            interpreterException.Message.Should().Contain("Expected a content type value of type 'string' but found 'integer' instead");
        }

        [Fact]
        public async Task VisitActivationExpression_GivenInvalidActivationExpressionWithRqlIntegerOnRuleName_ThrowsInterpreterExceptionExpectingRqlString()
        {
            // Arrange
            var expected = NewRqlArray();
            var ruleNameExpression = CreateMockedExpression(NewRqlAny(NewRqlInteger(1)));
            var contentTypeExpression = CreateMockedExpression(NewRqlAny(NewRqlString("Type1")));
            var activationExpression = new ActivationExpression(ruleNameExpression, contentTypeExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var interpreterException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitActivationExpression(activationExpression));

            // Assert
            interpreterException.Should().NotBeNull();
            interpreterException.Message.Should().Contain("Expected a rule name of type 'string' but found 'integer' instead");
        }

        [Fact]
        public async Task VisitActivationExpression_GivenValidActivationExpression_ProcessesRuleActivation()
        {
            // Arrange
            var expected = NewRqlArray();
            var ruleNameExpression = CreateMockedExpression(NewRqlString("Test rule name"));
            var contentTypeExpression = CreateMockedExpression(NewRqlString("Type1"));
            var activationExpression = new ActivationExpression(ruleNameExpression, contentTypeExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(r => r.ActivateRuleAsync(It.IsAny<ContentType>(), It.IsAny<string>()))
                .Returns(ValueTask.FromResult(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitActivationExpression(activationExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
            Mock.Get(runtime)
                .Verify(r => r.ActivateRuleAsync(It.IsAny<ContentType>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task VisitActivationExpression_GivenValidActivationExpressionFailingActivationOnRuntime_ThrowsInterpreterExceptionWithErrorMessageFromRuntime()
        {
            // Arrange
            var ruleNameExpression = CreateMockedExpression(NewRqlString("Test rule name"));
            var contentTypeExpression = CreateMockedExpression(NewRqlString("Type1"));
            var activationExpression = new ActivationExpression(ruleNameExpression, contentTypeExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            const string expected = "An error has occurred";
            Mock.Get(runtime)
                .Setup(r => r.ActivateRuleAsync(It.IsAny<ContentType>(), It.IsAny<string>()))
                .Throws(new RuntimeException(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var interpreterException = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitActivationExpression(activationExpression));

            // Assert
            interpreterException.Should().NotBeNull();
            interpreterException.Message.Should().Contain(expected);
        }

        [Fact]
        public async Task VisitActivationExpression_GivenValidActivationExpressionWithRqlAnyExpressionValues_ProcessesRuleActivation()
        {
            // Arrange
            var expected = NewRqlArray();
            var ruleNameExpression = CreateMockedExpression(NewRqlAny(NewRqlString("Test rule name")));
            var contentTypeExpression = CreateMockedExpression(NewRqlAny(NewRqlString("Type1")));
            var activationExpression = new ActivationExpression(ruleNameExpression, contentTypeExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(r => r.ActivateRuleAsync(It.IsAny<ContentType>(), It.IsAny<string>()))
                .Returns(ValueTask.FromResult(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitActivationExpression(activationExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
            Mock.Get(runtime)
                .Verify(r => r.ActivateRuleAsync(It.IsAny<ContentType>(), It.IsAny<string>()), Times.Once());
        }
    }
}