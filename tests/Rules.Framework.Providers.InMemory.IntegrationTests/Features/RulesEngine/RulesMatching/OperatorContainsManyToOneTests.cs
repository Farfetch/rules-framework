namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class OperatorContainsManyToOneTests : RulesEngineTestsBase
    {
        private static readonly ContentType testContentType = ContentType.ContentType1;
        private readonly Rule<ContentType, ConditionType> expectedMatchRule;
        private readonly Rule<ContentType, ConditionType> otherRule;

        public OperatorContainsManyToOneTests()
            : base(testContentType)
        {
            this.expectedMatchRule = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Expected rule")
                .WithDateBegin(UtcDate("2020-01-01Z"))
                .WithContent(testContentType, "Just as expected!")
                .WithCondition(ConditionType.ConditionType1, Operators.Contains, "Cat")
                .Build()
                .Rule;

            this.otherRule = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Other rule")
                .WithDateBegin(UtcDate("2020-01-01Z"))
                .WithContent(testContentType, "Oops! Not expected to be matched.")
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
            var emptyConditions = new[]
            {
                new Condition<ConditionType>(ConditionType.ConditionType1, new[]{ "Dog", "Fish", "Cat", "Spider", "Mockingbird", })
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
            var emptyConditions = new Condition<ConditionType>[]
            {
                new(ConditionType.ConditionType1, new[]{ "Dog", "Fish", "Bat", "Spider", "Mockingbird", })
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