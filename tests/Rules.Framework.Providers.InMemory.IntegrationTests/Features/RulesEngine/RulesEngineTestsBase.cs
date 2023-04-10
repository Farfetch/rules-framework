namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesEngine
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

        protected RulesEngineTestsBase(ContentType testContentType)
        {
            this.TestContentType = testContentType;

            this.RulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .WithContentType<ContentType>()
                .WithConditionType<ConditionType>()
                .SetInMemoryDataSource()
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();
        }

        protected RulesEngine<ContentType, ConditionType> RulesEngine { get; }

        protected void AddRules(IEnumerable<RuleSpecification> ruleSpecifications)
        {
            foreach (var ruleSpecification in ruleSpecifications)
            {
                this.RulesEngine.AddRuleAsync(
                    ruleSpecification.Rule,
                    ruleSpecification.RuleAddPriorityOption)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        protected async Task<Rule<ContentType, ConditionType>> MatchOneAsync(
            DateTime matchDate,
            Condition<ConditionType>[] conditions) => await RulesEngine.MatchOneAsync(
                TestContentType,
                matchDate,
                conditions)
            .ConfigureAwait(false);
    }
}