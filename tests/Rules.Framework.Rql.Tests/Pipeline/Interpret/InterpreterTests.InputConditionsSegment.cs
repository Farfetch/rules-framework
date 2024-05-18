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
            var expectedCondition1 = new Condition<ConditionType>(ConditionType.IsoCountryCode, "PT");
            var expectedCondition2 = new Condition<ConditionType>(ConditionType.IsVip, true);
            var inputConditionSegment1 = CreateMockedSegment(expectedCondition1);
            var inputConditionSegment2 = CreateMockedSegment(expectedCondition2);
            var inputConditionsSegment = new InputConditionsSegment(new[] { inputConditionSegment1, inputConditionSegment2 });

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitInputConditionsSegment(inputConditionsSegment);

            // Assert
            actual.Should().NotBeNull().And.BeAssignableTo<IEnumerable<Condition<ConditionType>>>();
            var actualConditions = actual as IEnumerable<Condition<ConditionType>>;
            actualConditions.Should().ContainInOrder(expectedCondition1, expectedCondition2);
        }
    }
}