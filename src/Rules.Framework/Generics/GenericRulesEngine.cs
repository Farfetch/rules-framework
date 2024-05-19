namespace Rules.Framework.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Extensions;

    internal sealed class GenericRulesEngine<TContentType, TConditionType> : IGenericRulesEngine
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;

        public GenericRulesEngine(IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        public async Task<RuleOperationResult> AddRuleAsync(Rule<string, string> rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var concreteRule = rule.ToConcreteRule<TContentType, TConditionType>();

            return await this.rulesEngine.AddRuleAsync(concreteRule, ruleAddPriorityOption).ConfigureAwait(false);
        }

        public IEnumerable<string> GetContentTypes()
        {
            return Enum.GetValues(typeof(TContentType))
               .Cast<object>()
               .Select(t => t.ToString());
        }

        public PriorityCriterias GetPriorityCriteria()
        {
            return this.rulesEngine.GetPriorityCriteria();
        }

        public async Task<IEnumerable<string>> GetUniqueConditionTypesAsync(string contentType, DateTime dateBegin, DateTime dateEnd)
        {
            var concreteContentType = contentType.ToConcreteContentType<TContentType>();

            var conditionTypes = await this.rulesEngine.GetUniqueConditionTypesAsync(concreteContentType, dateBegin, dateEnd).ConfigureAwait(false);

            return conditionTypes.Select(ct => ct!.ToString()).ToArray();
        }

        public async Task<IEnumerable<Rule<string, string>>> MatchManyAsync(string contentType, DateTime matchDateTime, IEnumerable<Condition<string>> conditions)
        {
            var concreteContentType = contentType.ToConcreteContentType<TContentType>();
            var concreteConditions = conditions.Select(c => new Condition<TConditionType>(c.Type.ToConcreteConditionType<TConditionType>(), c.Value)).ToArray();

            var concreteRules = await this.rulesEngine.MatchManyAsync(concreteContentType, matchDateTime, concreteConditions).ConfigureAwait(false);

            return concreteRules.Select(r => r.ToGenericRule()).ToArray();
        }

        public async Task<Rule<string, string>> MatchOneAsync(string contentType, DateTime matchDateTime, IEnumerable<Condition<string>> conditions)
        {
            var concreteContentType = contentType.ToConcreteContentType<TContentType>();
            var concreteConditions = conditions.Select(c => new Condition<TConditionType>(c.Type.ToConcreteConditionType<TConditionType>(), c.Value)).ToArray();

            var concreteRule = await this.rulesEngine.MatchOneAsync(concreteContentType, matchDateTime, concreteConditions).ConfigureAwait(false);

            return concreteRule.ToGenericRule();
        }

        public async Task<IEnumerable<Rule<string, string>>> SearchAsync(SearchArgs<string, string> searchArgs)
        {
            var innerSearchArgs = searchArgs.ToGenericSearchArgs<TContentType, TConditionType>();

            var result = await this.rulesEngine.SearchAsync(innerSearchArgs).ConfigureAwait(false);

            return result.Select(rule => rule.ToGenericRule()).ToArray();
        }

        public async Task<RuleOperationResult> UpdateRuleAsync(Rule<string, string> rule)
        {
            var concreteRule = rule.ToConcreteRule<TContentType, TConditionType>();

            return await this.rulesEngine.UpdateRuleAsync(concreteRule).ConfigureAwait(false);
        }
    }
}