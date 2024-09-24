namespace Rules.Framework.Providers.InMemory.IntegrationTests.Features.RulesEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Rules.Framework.Generic;
    using Rules.Framework.IntegrationTests.Common.Features;
    using Rules.Framework.Tests.Stubs;

    public abstract class RulesEngineTestsBase
    {
        private readonly ContentType TestContentType;

        protected RulesEngineTestsBase(ContentType testContentType)
        {
            this.TestContentType = testContentType;

            var compiledRulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(c =>
                {
                    c.EnableCompilation = true;
                    c.PriorityCriteria = PriorityCriterias.TopmostRuleWins;
                })
                .Build();
            this.CompiledRulesEngine = compiledRulesEngine.MakeGeneric<ContentType, ConditionType>();
            this.CompiledRulesEngine.CreateContentTypeAsync(testContentType).GetAwaiter().GetResult();

            var interpretedRulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();
            this.InterpretedRulesEngine = interpretedRulesEngine.MakeGeneric<ContentType, ConditionType>();
            this.InterpretedRulesEngine.CreateContentTypeAsync(testContentType).GetAwaiter().GetResult();
        }

        protected IRulesEngine<ContentType, ConditionType> CompiledRulesEngine { get; }

        protected IRulesEngine<ContentType, ConditionType> InterpretedRulesEngine { get; }

        protected async Task<OperationResult> ActivateRuleAsync(Rule<ContentType, ConditionType> rule, bool compiled)
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

        protected async Task<OperationResult> DeactivateRuleAsync(Rule<ContentType, ConditionType> rule, bool compiled)
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

        protected async Task<OperationResult> UpdateRuleAsync(Rule<ContentType, ConditionType> rule, bool compiled)
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