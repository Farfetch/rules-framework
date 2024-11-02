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
        public static IEnumerable<object[]> ValidCasesSearchExpression => new[]
        {
            new object[] { NewRqlString("Type1"), true },
            new object[] { NewRqlAny(NewRqlString("Type1")), true },
            new object[] { NewRqlString("Type1"), false },
            new object[] { NewRqlAny(NewRqlString("Type1")), false },
        };

        [Fact]
        public async Task VisitSearchExpression_GivenInvalidSearchExpressionWithInvalidRuleset_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var rulesetExpression = CreateMockedExpression(NewRqlDecimal(1m));
            var dateBeginExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var dateEndExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 12, 31)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var searchExpression = new SearchExpression(rulesetExpression, dateBeginExpression, dateEndExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitSearchExpression(searchExpression));

            // Act
            actual.Message.Should().Contain("Expected a ruleset value of type 'string' but found 'decimal' instead");
        }

        [Fact]
        public async Task VisitSearchExpression_GivenInvalidSearchExpressionWithUnknownRuleset_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var rulesetExpression = CreateMockedExpression(NewRqlString("dummy"));
            var dateBeginExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var dateEndExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 12, 31)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var searchExpression = new SearchExpression(rulesetExpression, dateBeginExpression, dateEndExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("other", DateTime.UtcNow))));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitSearchExpression(searchExpression));

            // Act
            actual.Message.Should().Contain("The ruleset 'dummy' was not found");
        }

        [Fact]
        public async Task VisitSearchExpression_GivenSearchExpressionFailingRuntimeEvaluation_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var rulesetExpression = CreateMockedExpression(NewRqlString("Type1"));
            var dateBeginExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var dateEndExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 12, 31)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var searchExpression = new SearchExpression(rulesetExpression, dateBeginExpression, dateEndExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.SearchRulesAsync(It.IsAny<SearchRulesArgs>()))
                .Throws(new RuntimeException("test"));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitSearchExpression(searchExpression));

            // Act
            actual.Message.Should().Contain("test");
        }

        [Theory]
        [MemberData(nameof(ValidCasesSearchExpression))]
        public async Task VisitSearchExpression_GivenValidSearchExpressionForOneCardinality_ReturnsRqlArrayWithOneRule(
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

            var rulesetExpression = CreateMockedExpression((IRuntimeValue)rulesetName);
            var dateBeginExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 1, 1)));
            var dateEndExpression = CreateMockedExpression(NewRqlDate(new DateTime(2024, 12, 31)));
            var inputConditionsSegment = CreateMockedSegment(conditions!);
            var searchExpression = new SearchExpression(rulesetExpression, dateBeginExpression, dateEndExpression, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.SearchRulesAsync(It.IsAny<SearchRulesArgs>()))
                .Returns(new ValueTask<RqlArray>(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitSearchExpression(searchExpression);

            // Act
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}