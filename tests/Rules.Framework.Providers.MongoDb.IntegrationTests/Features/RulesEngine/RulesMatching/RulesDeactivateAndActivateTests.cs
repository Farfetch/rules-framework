namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Generic;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesDeactivateAndActivateTests : RulesEngineTestsBase
    {
        private static readonly string rule1Name = "DummyRule1";
        private static readonly string rule1Value = "DummyRule1 Value";
        private static readonly string rule2Name = "DummyRule2";
        private static readonly string rule2Value = "DummyRule2 Value";
        private static readonly DateTime ruleEndDate = new DateTime(2021, 02, 01);
        private static readonly DateTime ruleStartDate = new DateTime(2020, 01, 01);
        private static readonly RulesetNames testRuleset = RulesetNames.Sample1;
        private readonly Rule<RulesetNames, ConditionNames> rule1;
        private readonly Rule<RulesetNames, ConditionNames> rule2;

        public RulesDeactivateAndActivateTests() : base(testRuleset)
        {
            rule1 = Rule.Create<RulesetNames, ConditionNames>(rule1Name)
                .InRuleset(testRuleset)
                .SetContent(rule1Value)
                .Since(ruleStartDate)
                .Until(ruleEndDate)
                .Build().Rule;

            rule2 = Rule.Create<RulesetNames, ConditionNames>(rule2Name)
                .InRuleset(testRuleset)
                .SetContent(rule2Value)
                .Since(ruleStartDate)
                .Until(ruleEndDate)
                .Build().Rule;

            this.AddRules(this.CreateTestRules());
        }

        [Fact]
        public async Task RulesEngine_DeactivateThenActivateRule_Validations()
        {
            // Arrange
            var emptyConditions = Array.Empty<Condition<ConditionNames>>();
            var matchDate = new DateTime(2020, 01, 02);

            // Act 1: Deactivate the rule
            var deactivateResult = await this.RulesEngine.DeactivateRuleAsync(rule1);

            // Assert 1: Rule 2 must be found
            Assert.True(deactivateResult.IsSuccess);
            var actualMatch1 = await this.MatchOneAsync(matchDate, emptyConditions);
            Assert.NotNull(actualMatch1);
            Assert.Equal(rule2.Name, actualMatch1.Name);

            // Act 2: Activate the rule
            var activateResult = await this.RulesEngine.ActivateRuleAsync(rule1);

            // Assert 2: Rule 1 must be found
            Assert.True(activateResult.IsSuccess);
            var actualMatch2 = await this.MatchOneAsync(matchDate, emptyConditions);
            Assert.NotNull(actualMatch2);
            Assert.Equal(rule1.Name, actualMatch2.Name);
        }

        private IEnumerable<RuleSpecification> CreateTestRules()
        {
            var ruleSpecs = new List<RuleSpecification>
            {
                new RuleSpecification(rule1, RuleAddPriorityOption.ByPriorityNumber(1)),
                new RuleSpecification(rule2, RuleAddPriorityOption.ByPriorityNumber(2))
            };

            return ruleSpecs;
        }
    }
}