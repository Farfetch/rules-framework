namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesInSequenceTests : RulesEngineTestsBase
    {
        private static readonly string rule1Name = "DummyRule1";
        private static readonly DateTime rule1StartDate = new DateTime(2020, 01, 01);
        private static readonly string rule1Value = "DummyRule1 Value";
        private static readonly DateTime rule2EndDate = new DateTime(2021, 02, 01);
        private static readonly string rule2Name = "DummyRule2";
        private static readonly string rule2Value = "DummyRule2 Value";
        private static readonly DateTime ruleChangeDate = new DateTime(2020, 07, 01, 14, 30, 00);
        private static readonly RulesetNames testRuleset = RulesetNames.Sample1;

        public RulesInSequenceTests() : base(testRuleset)
        {
            this.AddRules(CreateTestRules());
        }

        public static IEnumerable<object[]> FailureCases =>
            new List<object[]>
            {
                new object[] { rule1StartDate.AddMilliseconds(-1) }, // before 1st rule
                new object[] { rule2EndDate }, // at rules end
            };

        public static IEnumerable<object[]> SuccessCases =>
            new List<object[]>
            {
                new object[] { rule1StartDate, rule1Name, rule1Value }, // 1st rule
                new object[] { ruleChangeDate.AddMilliseconds(-1), rule1Name, rule1Value }, // immediatly before change
                new object[] { ruleChangeDate, rule2Name, rule2Value }, // 2nd rule
                new object[] { rule2EndDate.AddMilliseconds(-1), rule2Name, rule2Value }, // immediatly before rules end
            };

        [Theory]
        [MemberData(nameof(FailureCases))]
        public async Task RulesEngine_MatchOneAsync_OutsideRulesPeriod_Failure(DateTime matchDate)
        {
            // Arrange
            var emptyConditions = Array.Empty<Condition<ConditionNames>>();

            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, emptyConditions);

            // Assert
            Assert.Null(actualMatch);
        }

        [Theory]
        [MemberData(nameof(SuccessCases))]
        public async Task RulesEngine_MatchOneAsync_WithRulesInSequence_ReturnsCorrectRule(DateTime matchDate, string expectedName, string expectedValue)
        {
            // Arrange
            var emptyConditions = Array.Empty<Condition<ConditionNames>>();

            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, emptyConditions);

            // Assert
            Assert.Equal(expectedName, actualMatch.Name);
            Assert.Equal(testRuleset, actualMatch.Ruleset);
            Assert.Equal(expectedValue, actualMatch.ContentContainer.GetContentAs<string>());
        }

        private static List<RuleSpecification> CreateTestRules()
        {
            var ruleSpecs = new List<RuleSpecification>();

            var rule1 = Rule.Create<RulesetNames, ConditionNames>(rule1Name)
                .OnRuleset(testRuleset)
                .SetContent(rule1Value)
                .Since(rule1StartDate)
                .Until(ruleChangeDate)
                .Build();

            ruleSpecs.Add(new RuleSpecification(rule1.Rule, RuleAddPriorityOption.ByPriorityNumber(1)));

            var rule2 = Rule.Create<RulesetNames, ConditionNames>(rule2Name)
                .OnRuleset(testRuleset)
                .SetContent(rule2Value)
                .Since(ruleChangeDate)
                .Until(rule2EndDate)
                .Build();

            ruleSpecs.Add(new RuleSpecification(rule2.Rule, RuleAddPriorityOption.ByPriorityNumber(2)));

            return ruleSpecs;
        }
    }
}