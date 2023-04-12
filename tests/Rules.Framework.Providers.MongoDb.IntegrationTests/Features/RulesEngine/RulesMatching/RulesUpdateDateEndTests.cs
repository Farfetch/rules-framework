namespace Rules.Framework.Providers.MongoDb.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesUpdateDateEndTests : RulesEngineTestsBase
    {
        private static readonly string rule1Name = "DummyRule1";
        private static readonly string rule1Value = "DummyRule1 Value";
        private static readonly string rule2Name = "DummyRule2";
        private static readonly string rule2Value = "DummyRule2 Value";
        private static readonly DateTime ruleEndDate = new DateTime(2021, 02, 01);
        private static readonly DateTime ruleStartDate = new DateTime(2020, 01, 01);
        private static readonly ContentType TestContentType = ContentType.ContentType1;
        private readonly Rule<ContentType, ConditionType> rule1;
        private readonly Rule<ContentType, ConditionType> rule2;

        public RulesUpdateDateEndTests() : base(TestContentType)
        {
            rule1 = RuleBuilder
                .NewRule<ContentType, ConditionType>()
                .WithName(rule1Name)
                .WithContent(TestContentType, rule1Value)
                .WithDatesInterval(ruleStartDate, ruleEndDate)
                .Build().Rule;

            rule2 =
                RuleBuilder
                .NewRule<ContentType, ConditionType>()
                .WithName(rule2Name)
                .WithContent(TestContentType, rule2Value)
                .WithDatesInterval(ruleStartDate, ruleEndDate)
                .Build().Rule;

            this.AddRules(this.CreateTestRules());
        }

        public static IEnumerable<object[]> Cases =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 01, 01), true },
                new object[] { new DateTime(2020, 01, 01).AddMilliseconds(-1), false },
            };

        [Theory]
        [MemberData(nameof(Cases))]
        public async Task RulesEngine_UpdateRuleDateEnd_Validations(DateTime dateEnd, bool success)
        {
            // Arrange
            var emptyConditions = Array.Empty<Condition<ConditionType>>();
            var matchDate = new DateTime(2020, 01, 02);

            // Act
            rule1.DateEnd = dateEnd;
            var updateResult = await this.RulesEngine.UpdateRuleAsync(rule1);

            // Assert
            Assert.Equal(success, updateResult.IsSuccess);
            if (success)
            {
                var actualMatch1 = await this.MatchOneAsync(matchDate, emptyConditions).ConfigureAwait(false);
                Assert.NotNull(actualMatch1);
                Assert.Equal(rule2.Name, actualMatch1.Name);
            }
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