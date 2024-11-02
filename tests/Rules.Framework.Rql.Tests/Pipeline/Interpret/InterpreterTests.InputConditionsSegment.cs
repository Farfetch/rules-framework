namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System.Collections.Generic;
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
        public async Task VisitInputConditionsSegment_GivenValidInputConditionsSegment_ReturnsConditionsCollection()
        {
            // Arrange
            var expectedCondition1 = new ValueTuple<string, object>(nameof(Conditions.IsoCountryCode), "PT");
            var expectedCondition2 = new ValueTuple<string, object>(nameof(Conditions.IsVip), true);
            var inputConditionSegment1 = CreateMockedSegment(expectedCondition1);
            var inputConditionSegment2 = CreateMockedSegment(expectedCondition2);
            var inputConditionsSegment = new InputConditionsSegment(new[] { inputConditionSegment1, inputConditionSegment2 });

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitInputConditionsSegment(inputConditionsSegment);

            // Assert
            actual.Should().NotBeNull().And.BeAssignableTo<IDictionary<string, object>>();
            var actualConditions = actual as IDictionary<string, object>;
            actualConditions.Should()
                .Contain(expectedCondition1.Item1, expectedCondition1.Item2)
                .And
                .Contain(expectedCondition2.Item1, expectedCondition2.Item2);
        }
    }
}