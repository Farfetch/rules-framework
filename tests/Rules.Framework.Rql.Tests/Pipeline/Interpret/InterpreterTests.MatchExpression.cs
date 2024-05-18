namespace Rules.Framework.Rql.Tests.Pipeline.Interpret
{
    using System;
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
        public static IEnumerable<object[]> ValidCasesMatchExpression => new[]
        {
            new object[] { "one", NewRqlString("Type1"), true },
            new object[] { "one", NewRqlAny(NewRqlString("Type1")), true },
            new object[] { "one", NewRqlString("Type1"), false },
            new object[] { "one", NewRqlAny(NewRqlString("Type1")), false },
            new object[] { "all", NewRqlString("Type1"), true },
            new object[] { "all", NewRqlAny(NewRqlString("Type1")), true },
            new object[] { "all", NewRqlString("Type1"), false },
            new object[] { "one", NewRqlAny(NewRqlString("Type1")), false },
        };

        [Fact]
        public async Task VisitMatchExpression_GivenInvalidMatchExpressionWithInvalidContentType_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new[] { new Condition<ConditionType>(ConditionType.IsVip, false) };

            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var contentTypeExpression = CreateMockedExpression(NewRqlDecimal(1m));
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, contentTypeExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("Expected a content type value of type 'string' but found 'decimal' instead");
        }

        [Fact]
        public async Task VisitMatchExpression_GivenInvalidMatchExpressionWithUnknownContentType_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new[] { new Condition<ConditionType>(ConditionType.IsVip, false) };

            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var contentTypeExpression = CreateMockedExpression(NewRqlString("dummy"));
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, contentTypeExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("The content type value 'dummy' was not found");
        }

        [Fact]
        public async Task VisitMatchExpression_GivenMatchExpressionFailingRuntimeEvaluation_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new[] { new Condition<ConditionType>(ConditionType.IsVip, false) };

            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var contentTypeExpression = CreateMockedExpression(NewRqlString("Type1"));
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, contentTypeExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(x => x.MatchRulesAsync(It.IsAny<MatchRulesArgs<ContentType, ConditionType>>()))
                .Throws(new RuntimeException("test"));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("test");
        }

        [Theory]
        [MemberData(nameof(ValidCasesMatchExpression))]
        public async Task VisitMatchExpression_GivenValidMatchExpressionForOneCardinality_ReturnsOneRule(
            string cardinalityName,
            object contentTypeName,
            bool hasConditions)
        {
            // Arrange
            var ruleResult = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Dummy rule")
                .WithDateBegin(DateTime.Now)
                .WithContent(ContentType.Type1, "test")
                .WithCondition(x => x.Value(ConditionType.IsVip, Framework.Core.Operators.Equal, false))
                .Build();
            var conditions = hasConditions ? new[] { new Condition<ConditionType>(ConditionType.IsVip, false) } : null;

            var expected = new RqlArray(1);
            expected.SetAtIndex(0, new RqlRule<ContentType, ConditionType>(ruleResult.Rule));

            var cardinalitySegment = CreateMockedSegment(NewRqlString(cardinalityName));
            var contentTypeExpression = CreateMockedExpression((IRuntimeValue)contentTypeName);
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, contentTypeExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime<ContentType, ConditionType>>();
            Mock.Get(runtime)
                .Setup(x => x.MatchRulesAsync(It.IsAny<MatchRulesArgs<ContentType, ConditionType>>()))
                .Returns(new ValueTask<RqlArray>(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter<ContentType, ConditionType>(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitMatchExpression(matchExpression);

            // Act
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}