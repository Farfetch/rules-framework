namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitInputConditionSegment_GivenInvalidConditionType_ThrowsInterpreterException()
        {
            // Arrange
            var expectedRql = "@Dummy is \"test\"";
            var conditionValue = "test";
            var leftExpression = CreateMockedExpression(NewRqlString("Dummy"));
            var operatorToken = NewToken("is", null, Framework.Rql.Tokens.TokenType.IS);
            var rightExpression = CreateMockedExpression(NewRqlString(conditionValue));
            var inputConditionSegment = new InputConditionSegment(leftExpression, operatorToken, rightExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();
            Mock.Get(reverseRqlBuilder)
                .Setup(x => x.BuildRql(It.IsIn(inputConditionSegment)))
                .Returns(expectedRql);

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitInputConditionSegment(inputConditionSegment));

            // Assert
            actual.Should().NotBeNull();
            actual.Message.Should().Contain("Condition type of name '<string> \"Dummy\"' was not found.");
            actual.Rql.Should().Be(expectedRql);
        }

        [Fact]
        public async Task VisitInputConditionSegment_GivenValidInputConditionSegment_ReturnsCondition()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedConditionValue = "test";
            var leftExpression = CreateMockedExpression(NewRqlString("IsoCountryCode"));
            var operatorToken = NewToken("is", null, Framework.Rql.Tokens.TokenType.IS);
            var rightExpression = CreateMockedExpression(NewRqlString(expectedConditionValue));
            var inputConditionSegment = new InputConditionSegment(leftExpression, operatorToken, rightExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitInputConditionSegment(inputConditionSegment);

            // Assert
            actual.Should().NotBeNull().And.BeOfType<Condition<ConditionType>>();
            var actualCondition = actual as Condition<ConditionType>;
            actualCondition.Type.Should().Be(expectedConditionType);
            actualCondition.Value.Should().Be(expectedConditionValue);
        }
    }
}