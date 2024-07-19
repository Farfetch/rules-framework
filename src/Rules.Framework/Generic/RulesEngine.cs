namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Builder.Validation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public class RulesEngine<TContentType, TConditionType> : IRulesEngine<TContentType, TConditionType>
    {
        private readonly GenericRuleValidator<TContentType, TConditionType> ruleValidator = GenericRuleValidator<TContentType, TConditionType>.Instance;
        private readonly IRulesEngine wrappedRulesEngine;

        internal RulesEngine(IRulesEngine wrappedRulesEngine)
        {
            this.wrappedRulesEngine = wrappedRulesEngine;
        }

        public IRulesEngineOptions Options => this.wrappedRulesEngine.Options;

        /// <summary>
        /// Activates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public Task<RuleOperationResult> ActivateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return this.wrappedRulesEngine.ActivateRuleAsync(rule);
        }

        /// <summary>
        /// Adds a new rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="ruleAddPriorityOption">The rule add priority option.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule or rule</exception>
        /// <exception cref="NotSupportedException">
        /// The placement option '{ruleAddPriorityOption.PriorityOption}' is not supported.
        /// </exception>
        public Task<RuleOperationResult> AddRuleAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (ruleAddPriorityOption is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return this.wrappedRulesEngine.AddRuleAsync(rule, ruleAddPriorityOption);
        }

        /// <summary>
        /// Deactivates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public Task<RuleOperationResult> DeactivateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return this.wrappedRulesEngine.DeactivateRuleAsync(rule);
        }

        public async Task<IEnumerable<TContentType>> GetContentTypesAsync()
        {
            var contentTypes = await this.wrappedRulesEngine.GetContentTypesAsync().ConfigureAwait(false);

            return contentTypes.Select(x => GenericConversions.Convert<TContentType>(x)).ToArray();
        }

        /// <summary>
        /// Get the unique condition types associated with rules of a specific content type.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>All rules matching supplied conditions are returned.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, empty.</returns>
        public async Task<IEnumerable<TConditionType>> GetUniqueConditionTypesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            var contentTypeAsString = GenericConversions.Convert(contentType);
            var conditionTypes = await this.wrappedRulesEngine.GetUniqueConditionTypesAsync(contentTypeAsString, dateBegin, dateEnd).ConfigureAwait(false);
            return conditionTypes.Select(t => GenericConversions.Convert<TConditionType>(t)).ToArray();
        }

        /// <summary>
        /// Provides all rule matches (if any) to the given content type at the specified <paramref
        /// name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="matchDateTime"></param>
        /// <param name="conditions"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>All rules matching supplied conditions are returned.</para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, null.</returns>
        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchManyAsync(
            TContentType contentType,
            DateTime matchDateTime,
            IEnumerable<Condition<TConditionType>> conditions)
        {
            var contentTypeAsString = GenericConversions.Convert(contentType);
            var rules = await this.wrappedRulesEngine.MatchManyAsync(
                contentTypeAsString,
                matchDateTime,
                conditions
                    .Select(c => new Condition<string>(GenericConversions.Convert(c.Type), c.Value))
                    .ToArray()).ConfigureAwait(false);

            return rules.Select(r => r.ToGenericRule<TContentType, TConditionType>()).ToArray();
        }

        /// <summary>
        /// Provides a rule match (if any) to the given content type at the specified <paramref
        /// name="matchDateTime"/> and satisfying the supplied <paramref name="conditions"/>.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="matchDateTime"></param>
        /// <param name="conditions"></param>
        /// <remarks>
        /// <para>
        /// A set of rules is requested to rules data source and all conditions are evaluated
        /// against them to provide a set of matches.
        /// </para>
        /// <para>
        /// If there's more than one match, a rule is selected based on the priority criteria and
        /// value: topmost selects the lowest priority number and bottommost selects highest priority.
        /// </para>
        /// </remarks>
        /// <returns>the matched rule; otherwise, null.</returns>
        public async Task<Rule<TContentType, TConditionType>> MatchOneAsync(
            TContentType contentType,
            DateTime matchDateTime,
            IEnumerable<Condition<TConditionType>> conditions)
        {
            var contentTypeAsString = GenericConversions.Convert(contentType);
            var rule = await this.wrappedRulesEngine.MatchOneAsync(
                contentTypeAsString,
                matchDateTime,
                conditions
                    .Select(c => new Condition<string>(GenericConversions.Convert(c.Type), c.Value))
                    .ToArray()).ConfigureAwait(false);

            return rule?.ToGenericRule<TContentType, TConditionType>()!;
        }

        /// <summary>
        /// Searches for rules on given content type that match on supplied <paramref name="searchArgs"/>.
        /// </summary>
        /// <param name="searchArgs"></param>
        /// <remarks>
        /// <para>
        /// Only the condition types supplied on input conditions are evaluated, the remaining
        /// conditions are ignored.
        /// </para>
        /// </remarks>
        /// <returns>the set of rules matching the conditions.</returns>
        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> SearchAsync(SearchArgs<TContentType, TConditionType> searchArgs)
        {
            if (searchArgs is null)
            {
                throw new ArgumentNullException(nameof(searchArgs));
            }

            var contentTypeAsString = GenericConversions.Convert(searchArgs.ContentType);
            var searchArgsNew = new SearchArgs<string, string>(contentTypeAsString, searchArgs.DateBegin, searchArgs.DateEnd)
            {
                Conditions = searchArgs.Conditions.Select(c => new Condition<string>(GenericConversions.Convert(c.Type), c.Value)).ToArray(),
                ExcludeRulesWithoutSearchConditions = searchArgs.ExcludeRulesWithoutSearchConditions,
            };

            var rules = await this.wrappedRulesEngine.SearchAsync(searchArgsNew).ConfigureAwait(false);

            return rules.Select(r => r.ToGenericRule<TContentType, TConditionType>()).ToArray();
        }

        /// <summary>
        /// Updates the specified existing rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public Task<RuleOperationResult> UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return this.wrappedRulesEngine.UpdateRuleAsync(rule);
        }
    }
}