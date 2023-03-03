using Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesMatching;

namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesInSequenceTests : RulesEngineTestsBase
    {
        private static readonly DateTime rule1StartDate = new DateTime(2014, 11, 01);
        private static readonly DateTime rule2EndDate = new DateTime(2021, 12, 01);
        private static readonly DateTime ruleChangeDate = new DateTime(2021, 02, 01);
        private static readonly ContentType TestContentType = ContentType.ContentType1;

        public RulesInSequenceTests() : base(TestContentType)
        {
            this.AddRules(CreateTestRules());
        }

        public static IEnumerable<object[]> TestCase =>
            new List<object[]>
            {
                new object[] // 1st rule
                {
                    "DummyRule1",
                    rule1StartDate,
                    new Condition<ConditionType>[]
                    {
                        new Condition<ConditionType> { Type = ConditionType.ConditionType1, Value = 3 },
                        new Condition<ConditionType> { Type = ConditionType.ConditionType2, Value = "GBP" }
                    },
                    "DummyRule1 Value"
                },
                new object[] // frontier
                {
                    "DummyRule2",
                    ruleChangeDate,
                    new Condition<ConditionType>[]
                    {
                        new Condition<ConditionType> { Type = ConditionType.ConditionType1, Value = 3 },
                        new Condition<ConditionType> { Type = ConditionType.ConditionType2, Value = "GBP" }
                    },
                    "DummyRule2 value"
                },
                new object[] // 2nd rule
                {
                    "DummyRule2",
                    rule2EndDate.AddDays(-2), // .AddMilliseconds(-1),
                    new Condition<ConditionType>[]
                    {
                        new Condition<ConditionType> { Type = ConditionType.ConditionType1, Value = 3 },
                        new Condition<ConditionType> { Type = ConditionType.ConditionType2, Value = "GBP" }
                    },
                    "DummyRule2 Value"
                },
            };

        [Theory]
        [MemberData(nameof(TestCase))]
        public async Task RulesEngine_MatchOneAsync_RulesInSequence(
           string expectedRuleName,
           DateTime matchDate,
           Condition<ConditionType>[] expectedConditions,
           string expectedValue)
        {
            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, expectedConditions).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedRuleName, actualMatch.Name);
            Assert.Equal(TestContentType, actualMatch.ContentContainer.ContentType);
            Assert.Equal(expectedValue, actualMatch.ContentContainer.GetContentAs<string>());
        }

        private static IEnumerable<RuleSpecification> CreateTestRules()
        {
            var ruleSpecs = new List<RuleSpecification>();

            var rule1 =
                RuleBuilder
                .NewRule<ContentType, ConditionType>()
                .WithName("DummyRule1")
                .WithContent(TestContentType, "DummyRule1 Value")
                .WithDatesInterval(rule1StartDate, ruleChangeDate) //.AddMilliseconds(-1))
                .WithCondition(x => x
                    .AsComposed()
                    .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(z => z
                            .AsValued(ConditionType.ConditionType1).OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3).Build()
                        )
                        .AddCondition(z => z
                            .AsValued(ConditionType.ConditionType2).OfDataType<string>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand("GBP").Build()
                        )
                    .Build()
                )
                .Build();

            ruleSpecs.Add(new RuleSpecification(rule1, RuleAddPriorityOption.ByPriorityNumber(1)));

            var rule2 =
                RuleBuilder
                .NewRule<ContentType, ConditionType>()
                .WithName("DummyRule2")
                .WithContent(TestContentType, "DummyRule2 Value")
                .WithDatesInterval(ruleChangeDate, rule2EndDate)
                .WithCondition(x => x
                    .AsComposed()
                    .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(z => z
                            .AsValued(ConditionType.ConditionType1).OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3).Build()
                        )
                        .AddCondition(z => z
                            .AsValued(ConditionType.ConditionType2).OfDataType<string>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand("GBP").Build()
                        )
                    .Build()
                )
                .Build();

            ruleSpecs.Add(new RuleSpecification(rule2, RuleAddPriorityOption.ByPriorityNumber(2)));

            return ruleSpecs;
        }
    }
}