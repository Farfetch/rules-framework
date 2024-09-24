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

        /// <inheritdoc/>
        public IRulesEngineOptions Options => this.wrappedRulesEngine.Options;

        /// <inheritdoc/>
        public Task<OperationResult> ActivateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TContentType, TConditionType> to Rule.
            return this.wrappedRulesEngine.ActivateRuleAsync(rule);
        }

        /// <inheritdoc/>
        public Task<OperationResult> AddRuleAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TContentType, TConditionType> to Rule.
            return this.wrappedRulesEngine.AddRuleAsync(rule, ruleAddPriorityOption);
        }

        /// <inheritdoc/>
        public async Task CreateContentTypeAsync(TContentType contentType)
        {
            var contentTypeAsString = GenericConversions.Convert(contentType);
            await this.wrappedRulesEngine.CreateContentTypeAsync(contentTypeAsString).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<OperationResult> DeactivateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TContentType, TConditionType> to Rule.
            return this.wrappedRulesEngine.DeactivateRuleAsync(rule);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TContentType>> GetContentTypesAsync()
        {
            var contentTypes = await this.wrappedRulesEngine.GetContentTypesAsync().ConfigureAwait(false);

            return contentTypes.Select(x => GenericConversions.Convert<TContentType>(x)).ToArray();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TConditionType>> GetUniqueConditionTypesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            var contentTypeAsString = GenericConversions.Convert(contentType);
            var conditionTypes = await this.wrappedRulesEngine.GetUniqueConditionTypesAsync(contentTypeAsString, dateBegin, dateEnd).ConfigureAwait(false);
            return conditionTypes.Select(t => GenericConversions.Convert<TConditionType>(t)).ToArray();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Task<OperationResult> UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TContentType, TConditionType> to Rule.
            return this.wrappedRulesEngine.UpdateRuleAsync(rule);
        }
    }
}