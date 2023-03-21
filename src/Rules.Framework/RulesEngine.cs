namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Extensions;
    using Rules.Framework.Management;
    using Rules.Framework.Source;
    using Rules.Framework.Validation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public class RulesEngine<TContentType, TConditionType> : IRulesEngine<TContentType, TConditionType>
    {
        private readonly IConditionsEvalEngine<TConditionType> conditionsEvalEngine;
        private readonly IConditionTypeExtractor<TContentType, TConditionType> conditionTypeExtractor;
        private readonly RulesEngineOptions rulesEngineOptions;
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;
        private readonly RuleValidator<TContentType, TConditionType> ruleValidator = RuleValidator<TContentType, TConditionType>.Instance;
        private readonly IValidatorProvider validatorProvider;

        internal RulesEngine(
            IConditionsEvalEngine<TConditionType> conditionsEvalEngine,
            IRulesSource<TContentType, TConditionType> rulesSource,
            IValidatorProvider validatorProvider,
            RulesEngineOptions rulesEngineOptions,
            IConditionTypeExtractor<TContentType, TConditionType> conditionTypeExtractor)
        {
            this.conditionsEvalEngine = conditionsEvalEngine;
            this.rulesSource = rulesSource;
            this.validatorProvider = validatorProvider;
            this.rulesEngineOptions = rulesEngineOptions;
            this.conditionTypeExtractor = conditionTypeExtractor;
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

            return this.AddRuleInternalAsync(rule, ruleAddPriorityOption);
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

            rule.Active = false;

            var validationResult = this.ruleValidator.Validate(rule);

            if (!validationResult.IsValid)
            {
                return Task.FromResult(RuleOperationResult.Error(validationResult.Errors.Select(ve => ve.ErrorMessage)));
            }

            return this.UpdateRuleInternalAsync(rule);
        }

        /// <summary>
        /// Gets the priority criterias.
        /// </summary>
        /// <returns>Rules engine priority criterias</returns>
        public PriorityCriterias GetPriorityCriteria()
        {
            return this.rulesEngineOptions.PriorityCriteria;
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
            var getRulesArgs = new GetRulesArgs<TContentType>
            {
                ContentType = contentType,
                DateBegin = dateBegin,
                DateEnd = dateEnd,
            };

            var matchedRules = await this.rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

            return this.conditionTypeExtractor.GetConditionTypes(matchedRules);
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
        public Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchManyAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions)
        {
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var dateBegin = matchDateTime;
            var dateEnd = matchDateTime;

            return this.MatchAsync(contentType, dateBegin, dateEnd, conditions, evaluationOptions);
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
        public async Task<Rule<TContentType, TConditionType>> MatchOneAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions)
        {
            var matchedRules = await this.MatchManyAsync(contentType, matchDateTime, conditions).ConfigureAwait(false);

            return matchedRules.Any() ? this.SelectRuleByPriorityCriteria(matchedRules) : null;
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

            var validator = this.validatorProvider.GetValidatorFor<SearchArgs<TContentType, TConditionType>>();
            var validationResult = await validator.ValidateAsync(searchArgs).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                var stringBuilder = new StringBuilder()
                    .AppendFormat(CultureInfo.InvariantCulture, "Specified '{0}' with invalid search values:", nameof(searchArgs))
                    .AppendLine();

                foreach (var validationFailure in validationResult.Errors)
                {
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "> {0}", validationFailure.ErrorMessage)
                        .AppendLine();
                }

                throw new ArgumentException(stringBuilder.ToString(), nameof(searchArgs));
            }

            var dateBegin = searchArgs.DateBegin;
            var dateEnd = searchArgs.DateEnd;

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = searchArgs.ExcludeRulesWithoutSearchConditions,
                MatchMode = MatchModes.Search,
            };

            return await this.MatchAsync(searchArgs.ContentType, dateBegin, dateEnd, searchArgs.Conditions, evaluationOptions).ConfigureAwait(false);
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

            var validationResult = this.ruleValidator.Validate(rule);

            if (!validationResult.IsValid)
            {
                return Task.FromResult(RuleOperationResult.Error(validationResult.Errors.Select(ve => ve.ErrorMessage)));
            }

            return this.UpdateRuleInternalAsync(rule);
        }

        private async Task<RuleOperationResult> AddRuleInternalAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var errors = new List<string>();
            var rulesFilterArgs = new GetRulesFilteredArgs<TContentType>
            {
                ContentType = rule.ContentContainer.ContentType,
            };

            var existentRules = await this.rulesSource.GetRulesFilteredAsync(rulesFilterArgs).ConfigureAwait(false);

            if (ruleAddPriorityOption.PriorityOption == PriorityOptions.AtRuleName
                && !existentRules.Any(r => string.Equals(r.Name, ruleAddPriorityOption.AtRuleNameOptionValue, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"Rule name '{ruleAddPriorityOption.AtRuleNameOptionValue}' specified for priority placement does not exist.");
            }

            if (existentRules.Any(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"A rule with name '{rule.Name}' already exists.");
            }

            if (errors.Any())
            {
                return RuleOperationResult.Error(errors);
            }

            switch (ruleAddPriorityOption.PriorityOption)
            {
                case PriorityOptions.AtTop:
                    await this.AddRuleInternalAtTopAsync(rule, existentRules).ConfigureAwait(false);

                    break;

                case PriorityOptions.AtBottom:

                    await this.AddRuleInternalAtBottomAsync(rule, existentRules).ConfigureAwait(false);

                    break;

                case PriorityOptions.AtPriorityNumber:
                    await this.AddRuleInternalAtPriorityNumberAsync(rule, ruleAddPriorityOption, existentRules).ConfigureAwait(false);

                    break;

                case PriorityOptions.AtRuleName:
                    await this.AddRuleInternalAtRuleNameAsync(rule, ruleAddPriorityOption, existentRules).ConfigureAwait(false);

                    break;

                default:
                    throw new NotSupportedException($"The placement option '{ruleAddPriorityOption.PriorityOption}' is not supported.");
            }

            return RuleOperationResult.Success();
        }

        private async Task AddRuleInternalAtBottomAsync(Rule<TContentType, TConditionType> rule, IEnumerable<Rule<TContentType, TConditionType>> existentRules)
        {
            rule.Priority = !existentRules.Any() ? 1 : existentRules.Max(r => r.Priority) + 1;

            await ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .AddRule(rule)
                .ExecuteOperationsAsync()
                .ConfigureAwait(false);
        }

        private async Task AddRuleInternalAtPriorityNumberAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption, IEnumerable<Rule<TContentType, TConditionType>> existentRules)
        {
            var priorityMin = existentRules.MinOrDefault(r => r.Priority);
            var priorityMax = existentRules.MaxOrDefault(r => r.Priority);

            var rulePriority = ruleAddPriorityOption.AtPriorityNumberOptionValue;
            rulePriority = Math.Min(rulePriority, priorityMax + 1);
            rulePriority = Math.Max(rulePriority, priorityMin);

            rule.Priority = rulePriority;

            await ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .FilterFromThresholdPriorityToBottom(rulePriority)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync()
                .ConfigureAwait(false);
        }

        private async Task AddRuleInternalAtRuleNameAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption, IEnumerable<Rule<TContentType, TConditionType>> existentRules)
        {
            var firstPriorityToIncrement = existentRules
                                    .FirstOrDefault(r => string.Equals(r.Name, ruleAddPriorityOption.AtRuleNameOptionValue, StringComparison.OrdinalIgnoreCase))
                                    .Priority;
            rule.Priority = firstPriorityToIncrement;

            await ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .FilterFromThresholdPriorityToBottom(firstPriorityToIncrement)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync()
                .ConfigureAwait(false);
        }

        private async Task AddRuleInternalAtTopAsync(Rule<TContentType, TConditionType> rule, IEnumerable<Rule<TContentType, TConditionType>> existentRules)
        {
            rule.Priority = 1;

            await ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync()
                .ConfigureAwait(false);
        }

        private async Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchAsync(
            TContentType contentType,
            DateTime matchDateBegin,
            DateTime matchDateEnd,
            IEnumerable<Condition<TConditionType>> conditions,
            EvaluationOptions evaluationOptions)
        {
            var getRulesArgs = new GetRulesArgs<TContentType>
            {
                ContentType = contentType,
                DateBegin = matchDateBegin,
                DateEnd = matchDateEnd,
            };

            var rules = await this.rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

            var conditionsAsDictionary = conditions.ToDictionary(ks => ks.Type, ks => ks.Value);

            var matchedRules = rules
                .Where(r => r.RootCondition == null || this.conditionsEvalEngine.Eval(r.RootCondition, conditionsAsDictionary, evaluationOptions))
                .ToList();

            return matchedRules;
        }

        private Rule<TContentType, TConditionType> SelectRuleByPriorityCriteria(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            return this.rulesEngineOptions.PriorityCriteria == PriorityCriterias.BottommostRuleWins
                ? rules.OrderByDescending(r => r.Priority).First()
                : rules.OrderBy(r => r.Priority).First();
        }

        private async Task<RuleOperationResult> UpdateRuleInternalAsync(Rule<TContentType, TConditionType> rule)
        {
            var rulesFilterArgs = new GetRulesFilteredArgs<TContentType>
            {
                ContentType = rule.ContentContainer.ContentType,
            };

            var existentRules = await this.rulesSource.GetRulesFilteredAsync(rulesFilterArgs).ConfigureAwait(false);

            var existentRule = existentRules.FirstOrDefault(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
            if (existentRule is null)
            {
                return RuleOperationResult.Error(new[] { $"Rule with name '{rule.Name}' does not exist." });
            }

            var topPriorityThreshold = Math.Min(rule.Priority, existentRule.Priority);
            var bottomPriorityThreshold = Math.Max(rule.Priority, existentRule.Priority);

            switch (rule.Priority)
            {
                case int p when p > existentRule.Priority:
                    await ManagementOperations.Manage(existentRules)
                        .UsingSource(this.rulesSource)
                        .FilterPrioritiesRange(topPriorityThreshold, bottomPriorityThreshold)
                        .DecreasePriority()
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);
                    break;

                case int p when p < existentRule.Priority:
                    await ManagementOperations.Manage(existentRules)
                        .UsingSource(this.rulesSource)
                        .FilterPrioritiesRange(topPriorityThreshold, bottomPriorityThreshold)
                        .IncreasePriority()
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);
                    break;

                default:
                    await ManagementOperations.Manage(existentRules)
                        .UsingSource(this.rulesSource)
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);

                    break;
            }

            return RuleOperationResult.Success();
        }
    }
}