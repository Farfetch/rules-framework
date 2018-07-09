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

        internal RulesEngine(
            IConditionsEvalEngine<TConditionType> conditionsEvalEngine,
            IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.conditionsEvalEngine = conditionsEvalEngine;
            this.rulesDataSource = rulesDataSource;
        }

        public async Task<Rule<TContentType, TConditionType>> MatchOneAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions)
        {
            DateTime dateBegin = matchDateTime.Date;
            DateTime dateEnd = matchDateTime.Date.AddDays(1);

            IEnumerable<Rule<TContentType, TConditionType>> rules = await this.rulesDataSource.GetRules(contentType, dateBegin, dateEnd);

            IEnumerable<Rule<TContentType, TConditionType>> matchedRules = rules
                .Where(r => this.conditionsEvalEngine.Eval(r.RootCondition, conditions))
                .ToList();

            return matchedRules.Any() ? matchedRules.OrderBy(r => r.Priority).First() : null;
        }
    }
}