namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesMatching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;

    public abstract class RulesEngineTestsBase
    {
        private readonly ContentType TestContentType;

        internal RulesEngineTestsBase(ContentType testContentType)
        {
            this.TestContentType = testContentType;

            this.RulesEngine = RulesEngineBuilder
                    .CreateRulesEngine()
                    .WithContentType<ContentType>()
                    .WithConditionType<ConditionType>()
                    .SetInMemoryDataSource()
                    .Configure(c => c.PriotityCriteria = PriorityCriterias.TopmostRuleWins)
                    .Build();
        }

        protected RulesEngine<ContentType, ConditionType> RulesEngine { get; }

        protected void AddRules(IEnumerable<RuleSpecification> ruleSpecifications)
        {
            foreach (var ruleSpecification in ruleSpecifications)
            {
                RulesEngine.AddRuleAsync(
                    ruleSpecification.RuleBuilderResult.Rule,
                    ruleSpecification.RuleAddPriorityOption)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        protected async Task<Rule<ContentType, ConditionType>> MatchOneAsync(
            DateTime expectedMatchDate,
            Condition<ConditionType>[] expectedConditions) => await RulesEngine.MatchOneAsync(
                  TestContentType,
                  expectedMatchDate,
                  expectedConditions)
              .ConfigureAwait(false);
    }
}