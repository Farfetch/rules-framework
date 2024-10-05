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
        private readonly RulesetNames TestRuleset;

        protected RulesEngineTestsBase(RulesetNames testRuleset)
        {
            this.TestRuleset = testRuleset;

            var compiledRulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(c =>
                {
                    c.EnableCompilation = true;
                    c.PriorityCriteria = PriorityCriterias.TopmostRuleWins;
                })
                .Build();
            this.CompiledRulesEngine = compiledRulesEngine.MakeGeneric<RulesetNames, ConditionNames>();
            this.CompiledRulesEngine.CreateRulesetAsync(testRuleset).GetAwaiter().GetResult();

            var interpretedRulesEngine = RulesEngineBuilder
                .CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
                .Build();
            this.InterpretedRulesEngine = interpretedRulesEngine.MakeGeneric<RulesetNames, ConditionNames>();
            this.InterpretedRulesEngine.CreateRulesetAsync(testRuleset).GetAwaiter().GetResult();
        }

        protected IRulesEngine<RulesetNames, ConditionNames> CompiledRulesEngine { get; }

        protected IRulesEngine<RulesetNames, ConditionNames> InterpretedRulesEngine { get; }

        protected async Task<OperationResult> ActivateRuleAsync(Rule<RulesetNames, ConditionNames> rule, bool compiled)
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

        protected async Task<OperationResult> DeactivateRuleAsync(Rule<RulesetNames, ConditionNames> rule, bool compiled)
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

        protected async Task<Rule<RulesetNames, ConditionNames>> MatchOneAsync(
            DateTime matchDate,
            Condition<ConditionNames>[] conditions,
            bool compiled)
        {
            if (compiled)
            {
                return await CompiledRulesEngine.MatchOneAsync(TestRuleset, matchDate, conditions);
            }
            else
            {
                return await InterpretedRulesEngine.MatchOneAsync(TestRuleset, matchDate, conditions);
            }
        }

        protected async Task<OperationResult> UpdateRuleAsync(Rule<RulesetNames, ConditionNames> rule, bool compiled)
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