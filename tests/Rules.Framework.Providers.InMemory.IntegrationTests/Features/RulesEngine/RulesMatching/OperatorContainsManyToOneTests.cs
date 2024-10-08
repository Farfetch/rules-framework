namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.Generic;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class OperatorContainsManyToOneTests : RulesEngineTestsBase
    {
        private static readonly RulesetNames testContentType = RulesetNames.Sample1;
        private readonly Rule<RulesetNames, ConditionNames> expectedMatchRule;
        private readonly Rule<RulesetNames, ConditionNames> otherRule;

        public OperatorContainsManyToOneTests()
            : base(testContentType)
        {
            this.expectedMatchRule = Rule.Create<RulesetNames, ConditionNames>("Expected rule")
                .InRuleset(testContentType)
                .SetContent("Just as expected!")
                .Since(UtcDate("2020-01-01Z"))
                .ApplyWhen(ConditionNames.Condition1, Operators.Contains, "Cat")
                .Build()
                .Rule;

            this.otherRule = Rule.Create<RulesetNames, ConditionNames>("Other rule")
                .InRuleset(testContentType)
                .SetContent("Oops! Not expected to be matched.")
                .Since(UtcDate("2020-01-01Z"))
                .Build()
                .Rule;

            this.AddRules(this.CreateTestRules());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RulesEngine_GivenConditionType1WithArrayOfStringsContainingCat_MatchesExpectedRule(bool compiled)
        {
            // Arrange
            var emptyConditions = new Dictionary<ConditionNames, object>
            {
                {  ConditionNames.Condition1, new[]{ "Dog", "Fish", "Cat", "Spider", "Mockingbird", } },
            };
            var matchDate = UtcDate("2020-01-02Z");

            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, emptyConditions, compiled);

            // Assert
            actualMatch.Should().BeEquivalentTo(expectedMatchRule);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RulesEngine_GivenConditionType1WithArrayOfStringsNotContainingCat_MatchesOtherRule(bool compiled)
        {
            // Arrange
            var emptyConditions = new Dictionary<ConditionNames, object>
            {
                { ConditionNames.Condition1, new[]{ "Dog", "Fish", "Bat", "Spider", "Mockingbird", } },
            };
            var matchDate = UtcDate("2020-01-02Z");

            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, emptyConditions, compiled);

            // Assert
            actualMatch.Should().BeEquivalentTo(otherRule);
        }

        private IEnumerable<RuleSpecification> CreateTestRules()
        {
            var ruleSpecs = new List<RuleSpecification>
            {
                new RuleSpecification(expectedMatchRule, RuleAddPriorityOption.ByPriorityNumber(1)),
                new RuleSpecification(otherRule, RuleAddPriorityOption.ByPriorityNumber(2))
            };

            return ruleSpecs;
        }
    }
}