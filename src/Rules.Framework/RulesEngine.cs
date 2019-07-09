namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
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

        /// <summary>
        /// Provides a rule match (if any) to the given content type at the specified <paramref name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="matchDateTime"></param>
        /// <param name="conditions"></param>
        /// <remarks>
        /// <para>A set of rules is requested to rules data source and all conditions are evaluated against them to provide a set of matches.</para>
        /// <para>If there's more than one match, a rule is selected based on the priority criteria and value: topmost selects the lowest priority number and bottommost selects highest priority.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, null.</returns>
        public async Task<Rule<TContentType, TConditionType>> MatchOneAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions)
        {
            DateTime dateBegin = matchDateTime.Date;
            DateTime dateEnd = matchDateTime.Date.AddDays(1);

            IEnumerable<Rule<TContentType, TConditionType>> rules = await this.rulesDataSource.GetRulesAsync(contentType, dateBegin, dateEnd);

            IEnumerable<Rule<TContentType, TConditionType>> matchedRules = rules
                .Where(r => r.RootCondition == null || this.conditionsEvalEngine.Eval(r.RootCondition, conditions))
                .ToList();

            return matchedRules.Any() ? this.SelectRuleByPriorityCriteria(matchedRules) : null;
        }

        private Rule<TContentType, TConditionType> SelectRuleByPriorityCriteria(IEnumerable<Rule<TContentType, TConditionType>> rules)
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