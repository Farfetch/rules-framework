namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Extensions;
    using Rules.Framework.Management;
    using Rules.Framework.Rql;
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

        internal RulesEngine(RulesEngineArgs<TContentType, TConditionType> rulesEngineArgs)
        {
            this.conditionsEvalEngine = rulesEngineArgs.ConditionsEvalEngine;
            this.rulesSource = rulesEngineArgs.RulesSource;
            this.validatorProvider = rulesEngineArgs.ValidatorProvider;
            this.rulesEngineOptions = rulesEngineArgs.Options;
            this.conditionTypeExtractor = rulesEngineArgs.ConditionTypeExtractor;
        }

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

            rule.Active = true;

            return this.UpdateRuleInternalAsync(rule);
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

        public IRqlEngine<TContentType, TConditionType> GetRqlEngine()
        {
            return this.GetRqlEngine(RqlOptions.NewWithDefaults());
        }

        public IRqlEngine<TContentType, TConditionType> GetRqlEngine(RqlOptions rqlOptions)
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new NotSupportedException($"Rule Query Language is not supported for non-enum types of {nameof(TContentType)}.");
            }

            if (!typeof(TConditionType).IsEnum)
            {
                throw new NotSupportedException($"Rule Query Language is not supported for non-enum types of {nameof(TConditionType)}.");
            }

            return RqlEngineBuilder<TContentType, TConditionType>.CreateRqlEngine(this)
                .WithOptions(rqlOptions)
                .WithRulesSource(this.rulesSource)
                .Build();
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
        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchManyAsync(
            TContentType contentType,
            DateTime matchDateTime,
            IEnumerable<Condition<TConditionType>> conditions)
        {
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var getRulesArgs = new GetRulesArgs<TContentType>
            {
                ContentType = contentType,
                DateBegin = matchDateTime,
                DateEnd = matchDateTime,
            };

            var conditionsAsDictionary = conditions.ToDictionary(ks => ks.Type, ks => ks.Value);
            var orderedRules = await this.GetRulesOrderedAscendingAsync(getRulesArgs).ConfigureAwait(false);
            return this.EvalAll(orderedRules, evaluationOptions, conditionsAsDictionary, active: true);
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
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var getRulesArgs = new GetRulesArgs<TContentType>
            {
                ContentType = contentType,
                DateBegin = matchDateTime,
                DateEnd = matchDateTime,
            };

            var conditionsAsDictionary = conditions.ToDictionary(ks => ks.Type, ks => ks.Value);
            var orderedRules = await this.GetRulesOrderedAscendingAsync(getRulesArgs).ConfigureAwait(false);
            return this.rulesEngineOptions.PriorityCriteria == PriorityCriterias.TopmostRuleWins
                ? EvalOneTraverse(orderedRules, evaluationOptions, conditionsAsDictionary, active: true)
                : EvalOneReverse(orderedRules, evaluationOptions, conditionsAsDictionary, active: true);
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

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = searchArgs.ExcludeRulesWithoutSearchConditions,
                MatchMode = MatchModes.Search,
            };

            var getRulesArgs = new GetRulesArgs<TContentType>
            {
                ContentType = searchArgs.ContentType,
                DateBegin = searchArgs.DateBegin,
                DateEnd = searchArgs.DateEnd,
            };

            var conditionsAsDictionary = searchArgs.Conditions.ToDictionary(ks => ks.Type, ks => ks.Value);
            var orderedRules = await this.GetRulesOrderedAscendingAsync(getRulesArgs).ConfigureAwait(false);
            return this.EvalAll(orderedRules, evaluationOptions, conditionsAsDictionary, searchArgs.Active);
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

        private IEnumerable<Rule<TContentType, TConditionType>> EvalAll(
            List<Rule<TContentType, TConditionType>> orderedRules,
            EvaluationOptions evaluationOptions,
            Dictionary<TConditionType, object> conditionsAsDictionary,
            bool? active)
        {
            // Begins evaluation at the first element of the given list as parameter. Returns all
            // rules that match. Assumes given list is ordered.
            var matchedRules = new List<Rule<TContentType, TConditionType>>(orderedRules.Count);
            foreach (var rule in orderedRules)
            {
                if (this.EvalRule(rule, evaluationOptions, conditionsAsDictionary, active))
                {
                    matchedRules.Add(rule);
                }
            }

            return matchedRules.AsReadOnly();
        }

        private Rule<TContentType, TConditionType> EvalOneReverse(
            List<Rule<TContentType, TConditionType>> rules,
            EvaluationOptions evaluationOptions,
            Dictionary<TConditionType, object> conditionsAsDictionary,
            bool? active)
        {
            // Begins evaluation at the last element of the given list as parameter. Returns the
            // first rule that matches. Assumes given list is ordered.
            for (int i = rules.Count - 1; i >= 0; i--)
            {
                var rule = rules[i];
                if (this.EvalRule(rule, evaluationOptions, conditionsAsDictionary, active))
                {
                    return rule;
                }
            }

            return null!;
        }

        private Rule<TContentType, TConditionType> EvalOneTraverse(
            List<Rule<TContentType, TConditionType>> rules,
            EvaluationOptions evaluationOptions,
            Dictionary<TConditionType, object> conditionsAsDictionary,
            bool? active)
        {
            // Begins evaluation at the first element of the given list as parameter. Returns the
            // first rule that matches. Assumes given list is ordered.
            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                if (this.EvalRule(rule, evaluationOptions, conditionsAsDictionary, active))
                {
                    return rule;
                }
            }

            return null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EvalRule(
            Rule<TContentType, TConditionType> rule,
            EvaluationOptions evaluationOptions,
            Dictionary<TConditionType, object> conditionsAsDictionary,
            bool? active)
            => rule.Active == active.GetValueOrDefault(defaultValue: true) && (rule.RootCondition == null || this.conditionsEvalEngine.Eval(rule.RootCondition, conditionsAsDictionary, evaluationOptions));

        private async Task<List<Rule<TContentType, TConditionType>>> GetRulesOrderedAscendingAsync(GetRulesArgs<TContentType> getRulesArgs)
        {
            var rules = await this.rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);
            var orderedRules = new List<Rule<TContentType, TConditionType>>(rules.Count());
            var greatestPriority = 0;
            foreach (var rule in rules)
            {
                if (orderedRules.Count == 0 || rule.Priority > greatestPriority)
                {
                    orderedRules.Add(rule);
                    greatestPriority = rule.Priority;
                    continue;
                }

                for (int i = 0; i < orderedRules.Count; i++)
                {
                    var currentRule = orderedRules[i];
                    if (rule.Priority < currentRule.Priority)
                    {
                        orderedRules.Insert(i, rule);
                        break;
                    }
                }
            }

            return orderedRules;
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

            var validationResult = this.ruleValidator.Validate(rule);

            if (!validationResult.IsValid)
            {
                return RuleOperationResult.Error(validationResult.Errors.Select(ve => ve.ErrorMessage));
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