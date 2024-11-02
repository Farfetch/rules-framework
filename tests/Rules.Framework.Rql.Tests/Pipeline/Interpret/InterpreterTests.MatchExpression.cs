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
        public async Task VisitMatchExpression_GivenInvalidMatchExpressionWithInvalidRuleset_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var rulesetExpression = CreateMockedExpression(NewRqlDecimal(1m));
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, rulesetExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("Expected a ruleset value of type 'string' but found 'decimal' instead");
        }

        [Fact]
        public async Task VisitMatchExpression_GivenInvalidMatchExpressionWithUnknownRuleset_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var rulesetExpression = CreateMockedExpression(NewRqlString("dummy"));
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, rulesetExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("other", DateTime.UtcNow))));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("The ruleset 'dummy' was not found");
        }

        [Fact]
        public async Task VisitMatchExpression_GivenMatchExpressionFailingRuntimeEvaluation_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var rulesetExpression = CreateMockedExpression(NewRqlString("Type1"));
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(cardinalitySegment, rulesetExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.MatchRulesAsync(It.IsAny<MatchRulesArgs>()))
                .Throws(new RuntimeException("test"));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("test");
        }

        [Theory]
        [MemberData(nameof(ValidCasesMatchExpression))]
        public async Task VisitMatchExpression_GivenValidMatchExpressionForOneCardinality_ReturnsOneRule(
            string cardinalityName,
            object rulesetName,
            bool hasConditions)
        {
            // Arrange
            var ruleResult = Rule.Create<Rulesets, Conditions>("Dummy rule")
                .InRuleset(Rulesets.Type1)
                .SetContent("test")
                .Since(DateTime.Now)
                .ApplyWhen(x => x.Value(Conditions.IsVip, Operators.Equal, false))
                .Build();
            var conditions = hasConditions
                ? new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    { nameof(Conditions.IsVip), false },
                }
                : null;

            var expected = new RqlArray(1);
            expected.SetAtIndex(0, new RqlRule(ruleResult.Rule));

            var cardinalitySegment = CreateMockedSegment(NewRqlString(cardinalityName));
            var rulesetExpression = CreateMockedExpression((IRuntimeValue)rulesetName);
            var matchDateExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions!);
            var matchExpression = MatchExpression.Create(cardinalitySegment, rulesetExpression, matchDateExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.MatchRulesAsync(It.IsAny<MatchRulesArgs>()))
                .Returns(new ValueTask<RqlArray>(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitMatchExpression(matchExpression);

            // Act
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}