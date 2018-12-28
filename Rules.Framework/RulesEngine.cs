using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rules.Framework.Core;
using Rules.Framework.Evaluation;

namespace Rules.Framework
{
    public class RulesEngine<TContentType, TConditionType>
    {
        private readonly IConditionsEvalEngine<TConditionType> conditionsEvalEngine;

        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;

        private readonly RulesEngineOptions rulesEngineOptions;

        internal RulesEngine(
            IConditionsEvalEngine<TConditionType> conditionsEvalEngine,
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionsEvalEngine = conditionsEvalEngine;
            this.rulesDataSource = rulesDataSource;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public async Task<Rule<TContentType, TConditionType>> MatchOneAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions)
        {
            DateTime dateBegin = matchDateTime.Date;
            DateTime dateEnd = matchDateTime.Date.AddDays(1);

            IEnumerable<Rule<TContentType, TConditionType>> rules = await this.rulesDataSource.GetRulesAsync(contentType, dateBegin, dateEnd);

            IEnumerable<Rule<TContentType, TConditionType>> matchedRules = rules
                .Where(r => r.RootCondition != null ? this.conditionsEvalEngine.Eval(r.RootCondition, conditions) : true)
                .ToList();

            return matchedRules.Any() ? this.SelectRuleByPriorityCriteria(matchedRules) : null;
        }

        public Rule<TContentType, TConditionType> SelectRuleByPriorityCriteria(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            switch (this.rulesEngineOptions.PriotityCriteria)
            {
                case PriorityCriterias.BottommostRuleWins:
                    return rules.OrderByDescending(r => r.Priority).First();

                default:
                    return rules.OrderBy(r => r.Priority).First();
            }
        }
    }
}