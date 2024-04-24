namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

            this.CompiledRulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .WithContentType<ContentType>()
                .WithConditionType<ConditionType>()
                .SetInMemoryDataSource()
                .Configure(c =>
                {
                    c.EnableCompilation = true;
                    c.PriorityCriteria = PriorityCriterias.TopmostRuleWins;
                })
                .Build();

            this.InterpretedRulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .WithContentType<ContentType>()
                .WithConditionType<ConditionType>()
                .SetInMemoryDataSource()
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();
        }

        protected RulesEngine<ContentType, ConditionType> CompiledRulesEngine { get; }

        protected RulesEngine<ContentType, ConditionType> InterpretedRulesEngine { get; }

        protected async Task<RuleOperationResult> ActivateRuleAsync(Rule<ContentType, ConditionType> rule, bool compiled)
        {
            if (compiled)
            {
                return await CompiledRulesEngine.ActivateRuleAsync(rule);
            }
            else
            {
                return await InterpretedRulesEngine.ActivateRuleAsync(rule);
            }
        }

        protected void AddRules(IEnumerable<RuleSpecification> ruleSpecifications)
        {
            foreach (var ruleSpecification in ruleSpecifications)
            {
                this.CompiledRulesEngine.AddRuleAsync(ruleSpecification.Rule, ruleSpecification.RuleAddPriorityOption)
                    .GetAwaiter()
                    .GetResult();

                this.InterpretedRulesEngine.AddRuleAsync(ruleSpecification.Rule, ruleSpecification.RuleAddPriorityOption)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        protected async Task<RuleOperationResult> DeactivateRuleAsync(Rule<ContentType, ConditionType> rule, bool compiled)
        {
            if (compiled)
            {
                return await CompiledRulesEngine.DeactivateRuleAsync(rule);
            }
            else
            {
                return await InterpretedRulesEngine.DeactivateRuleAsync(rule);
            }
        }

        protected async Task<Rule<ContentType, ConditionType>> MatchOneAsync(
            DateTime matchDate,
            Condition<ConditionType>[] conditions,
            bool compiled)
        {
            if (compiled)
            {
                return await CompiledRulesEngine.MatchOneAsync(TestContentType, matchDate, conditions);
            }
            else
            {
                return await InterpretedRulesEngine.MatchOneAsync(TestContentType, matchDate, conditions);
            }
        }

        protected async Task<RuleOperationResult> UpdateRuleAsync(Rule<ContentType, ConditionType> rule, bool compiled)
        {
            if (compiled)
            {
                return await CompiledRulesEngine.UpdateRuleAsync(rule);
            }
            else
            {
                return await InterpretedRulesEngine.UpdateRuleAsync(rule);
            }
        }

        protected DateTime UtcDate(string date)
            => DateTime.Parse(date, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
    }
}