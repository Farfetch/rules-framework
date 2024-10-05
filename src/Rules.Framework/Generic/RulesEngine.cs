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
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public class RulesEngine<TRuleset, TCondition> : IRulesEngine<TRuleset, TCondition>
    {
        private readonly GenericRuleValidator<TRuleset, TCondition> ruleValidator = GenericRuleValidator<TRuleset, TCondition>.Instance;
        private readonly IRulesEngine wrappedRulesEngine;

        internal RulesEngine(IRulesEngine wrappedRulesEngine)
        {
            this.wrappedRulesEngine = wrappedRulesEngine;
        }

        /// <inheritdoc/>
        public IRulesEngineOptions Options => this.wrappedRulesEngine.Options;

        /// <inheritdoc/>
        public Task<OperationResult> ActivateRuleAsync(Rule<TRuleset, TCondition> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TRuleset, TCondition> to Rule.
            return this.wrappedRulesEngine.ActivateRuleAsync(rule);
        }

        /// <inheritdoc/>
        public Task<OperationResult> AddRuleAsync(Rule<TRuleset, TCondition> rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TRuleset, TCondition> to Rule.
            return this.wrappedRulesEngine.AddRuleAsync(rule, ruleAddPriorityOption);
            }

        /// <inheritdoc/>
        public async Task CreateRulesetAsync(TRuleset ruleset)
        {
            var rulesetAsString = GenericConversions.Convert(ruleset);
            await this.wrappedRulesEngine.CreateRulesetAsync(rulesetAsString).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<OperationResult> DeactivateRuleAsync(Rule<TRuleset, TCondition> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TRuleset, TCondition> to Rule.
            return this.wrappedRulesEngine.DeactivateRuleAsync(rule);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Ruleset<TRuleset>>> GetRulesetsAsync()
        {
            var rulesets = await this.wrappedRulesEngine.GetRulesetsAsync().ConfigureAwait(false);

            return rulesets.Select(x => new Ruleset<TRuleset>(x)).ToArray();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TCondition>> GetUniqueConditionsAsync(TRuleset ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var rulesetAsString = GenericConversions.Convert(ruleset);
            var conditions = await this.wrappedRulesEngine.GetUniqueConditionsAsync(rulesetAsString, dateBegin, dateEnd).ConfigureAwait(false);
            return conditions.Select(t => GenericConversions.Convert<TCondition>(t)).ToArray();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rule<TRuleset, TCondition>>> MatchManyAsync(
            TRuleset ruleset,
            DateTime matchDateTime,
            IEnumerable<Condition<TCondition>> conditions)
        {
            var rulesetAsString = GenericConversions.Convert(ruleset);
            var rules = await this.wrappedRulesEngine.MatchManyAsync(
                rulesetAsString,
                matchDateTime,
                conditions
                    .Select(c => new Condition<string>(GenericConversions.Convert(c.Type), c.Value))
                    .ToArray()).ConfigureAwait(false);

            return rules.Select(r => r.ToGenericRule<TRuleset, TCondition>()).ToArray();
        }

        /// <inheritdoc/>
        public async Task<Rule<TRuleset, TCondition>> MatchOneAsync(
            TRuleset ruleset,
            DateTime matchDateTime,
            IEnumerable<Condition<TCondition>> conditions)
        {
            var rulesetAsString = GenericConversions.Convert(ruleset);
            var rule = await this.wrappedRulesEngine.MatchOneAsync(
                rulesetAsString,
                matchDateTime,
                conditions
                    .Select(c => new Condition<string>(GenericConversions.Convert(c.Type), c.Value))
                    .ToArray()).ConfigureAwait(false);

            return rule?.ToGenericRule<TRuleset, TCondition>()!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rule<TRuleset, TCondition>>> SearchAsync(SearchArgs<TRuleset, TCondition> searchArgs)
        {
            if (searchArgs is null)
            {
                throw new ArgumentNullException(nameof(searchArgs));
            }

            var rulesetAsString = GenericConversions.Convert(searchArgs.Ruleset);
            var searchArgsNew = new SearchArgs<string, string>(rulesetAsString, searchArgs.DateBegin, searchArgs.DateEnd)
            {
                Conditions = searchArgs.Conditions.Select(c => new Condition<string>(GenericConversions.Convert(c.Type), c.Value)).ToArray(),
                ExcludeRulesWithoutSearchConditions = searchArgs.ExcludeRulesWithoutSearchConditions,
            };

            var rules = await this.wrappedRulesEngine.SearchAsync(searchArgsNew).ConfigureAwait(false);

            return rules.Select(r => r.ToGenericRule<TRuleset, TCondition>()).ToArray();
        }

        /// <inheritdoc/>
        public Task<OperationResult> UpdateRuleAsync(Rule<TRuleset, TCondition> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            // Implicit conversion from Rule<TRuleset, TCondition> to Rule.
            return this.wrappedRulesEngine.UpdateRuleAsync(rule);
        }
    }
}