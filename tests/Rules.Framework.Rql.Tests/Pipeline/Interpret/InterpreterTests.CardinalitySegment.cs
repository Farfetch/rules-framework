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
        public async Task VisitCardinalitySegment_GivenValidCardinalitySegment_ReturnsCardinalityValue()
        {
            // Arrange
            var expected = NewRqlString("ONE");
            var cardinalityExpression = CreateMockedExpression(expected);
            var ruleExpression = CreateMockedExpression(NewRqlString("rule"));
            var cardinalitySegment = CardinalitySegment.Create(cardinalityExpression, ruleExpression);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitCardinalitySegment(cardinalitySegment);

            // Assert
            actual.Should().NotBeNull().And.BeEquivalentTo(expected);
        }
    }
}