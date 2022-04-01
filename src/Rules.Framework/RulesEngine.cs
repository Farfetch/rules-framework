namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Management;
    using Rules.Framework.Validation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public class RulesEngine<TContentType, TConditionType>
    {
        private readonly IConditionsEvalEngine<TConditionType> conditionsEvalEngine;

        private readonly IConditionTypeExtractor<TContentType, TConditionType> conditionTypeExtractor;
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;
        private readonly RulesEngineOptions rulesEngineOptions;
        private readonly IValidatorProvider validatorProvider;

        internal RulesEngine(
            IConditionsEvalEngine<TConditionType> conditionsEvalEngine,
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            IValidatorProvider validatorProvider,
            RulesEngineOptions rulesEngineOptions,
            IConditionTypeExtractor<TContentType, TConditionType> conditionTypeExtractor)
        {
            this.conditionsEvalEngine = conditionsEvalEngine;
            this.rulesDataSource = rulesDataSource;
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
        /// Get the unique condition types associated with rules of a specific content type/>.
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
        /// <returns>the matched rule; otherwise, null.</returns>
        public async Task<IEnumerable<TConditionType>> GetUniqueConditionTypesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            var matchedRules = await this.rulesDataSource.GetRulesAsync(contentType, dateBegin, dateEnd).ConfigureAwait(false);

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
            EvaluationOptions evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact
            };

            DateTime dateBegin = matchDateTime.Date;
            DateTime dateEnd = matchDateTime.Date.AddDays(1);

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
            IEnumerable<Rule<TContentType, TConditionType>> matchedRules = await this.MatchManyAsync(contentType, matchDateTime, conditions).ConfigureAwait(false);

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
        public Task<IEnumerable<Rule<TContentType, TConditionType>>> SearchAsync(SearchArgs<TContentType, TConditionType> searchArgs)
        {
            if (searchArgs is null)
            {
                throw new ArgumentNullException(nameof(searchArgs));
            }

            IValidator<SearchArgs<TContentType, TConditionType>> validator = this.validatorProvider.GetValidatorFor<SearchArgs<TContentType, TConditionType>>();
            ValidationResult validationResult = validator.Validate(searchArgs);
            if (!validationResult.IsValid)
            {
                StringBuilder stringBuilder = new StringBuilder()
                    .AppendFormat("Specified '{0}' with invalid search values:", nameof(searchArgs))
                    .AppendLine();

                foreach (ValidationFailure validationFailure in validationResult.Errors)
                {
                    stringBuilder.AppendFormat("> {0}", validationFailure.ErrorMessage)
                        .AppendLine();
                }

                throw new ArgumentException(stringBuilder.ToString(), nameof(searchArgs));
            }

            DateTime dateBegin = searchArgs.DateBegin;
            DateTime dateEnd = searchArgs.DateEnd;

            EvaluationOptions evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = searchArgs.ExcludeRulesWithoutSearchConditions,
                MatchMode = MatchModes.Search
            };

            if (dateBegin == dateEnd)
            {
                dateEnd = dateBegin.AddDays(1);
            }

            return this.MatchAsync(searchArgs.ContentType, dateBegin, dateEnd, searchArgs.Conditions, evaluationOptions);
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
            List<string> errors = new List<string>();
            RulesFilterArgs<TContentType> rulesFilterArgs = new RulesFilterArgs<TContentType>
            {
                ContentType = rule.ContentContainer.ContentType
            };

            IEnumerable<Rule<TContentType, TConditionType>> existentRules = await this.rulesDataSource.GetRulesByAsync(rulesFilterArgs).ConfigureAwait(false);

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
                    rule.Priority = 1;

                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
                        .IncreasePriority()
                        .UpdateRules()
                        .AddRule(rule)
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);

                    break;

                case PriorityOptions.AtBottom:

                    rule.Priority = !existentRules.Any() ? 1 : existentRules.Max(r => r.Priority) + 1;

                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
                        .AddRule(rule)
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);

                    break;

                case PriorityOptions.AtPriorityNumber:
                    int priorityMin = existentRules.MinOrDefault(r => r.Priority);
                    int priorityMax = existentRules.MaxOrDefault(r => r.Priority);

                    int rulePriority = ruleAddPriorityOption.AtPriorityNumberOptionValue;
                    rulePriority = Math.Min(rulePriority, priorityMax + 1);
                    rulePriority = Math.Max(rulePriority, priorityMin);

                    rule.Priority = rulePriority;

                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
                        .FilterFromThresholdPriorityToBottom(rulePriority)
                        .IncreasePriority()
                        .UpdateRules()
                        .AddRule(rule)
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);

                    break;

                case PriorityOptions.AtRuleName:
                    int firstPriorityToIncrement = existentRules
                        .FirstOrDefault(r => string.Equals(r.Name, ruleAddPriorityOption.AtRuleNameOptionValue, StringComparison.OrdinalIgnoreCase))
                        .Priority;
                    rule.Priority = firstPriorityToIncrement;

                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
                        .FilterFromThresholdPriorityToBottom(firstPriorityToIncrement)
                        .IncreasePriority()
                        .UpdateRules()
                        .AddRule(rule)
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);

                    break;

                default:
                    throw new NotSupportedException($"The placement option '{ruleAddPriorityOption.PriorityOption}' is not supported.");
            }

            return RuleOperationResult.Success();
        }

        private async Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchAsync(
            TContentType contentType,
            DateTime matchDateBegin,
            DateTime matchDateEnd,
            IEnumerable<Condition<TConditionType>> conditions,
            EvaluationOptions evaluationOptions)
        {
            IEnumerable<Rule<TContentType, TConditionType>> rules = await this.rulesDataSource.GetRulesAsync(contentType, matchDateBegin, matchDateEnd).ConfigureAwait(false);

            IEnumerable<Rule<TContentType, TConditionType>> matchedRules = rules
                .Where(r => r.RootCondition == null || this.conditionsEvalEngine.Eval(r.RootCondition, conditions, evaluationOptions))
                .ToList();

            return matchedRules;
        }

        private Rule<TContentType, TConditionType> SelectRuleByPriorityCriteria(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            return this.rulesEngineOptions.PriotityCriteria switch
            {
                PriorityCriterias.BottommostRuleWins => rules.OrderByDescending(r => r.Priority).First(),
                _ => rules.OrderBy(r => r.Priority).First(),
            };
        }

        private async Task<RuleOperationResult> UpdateRuleInternalAsync(Rule<TContentType, TConditionType> rule)
        {
            RulesFilterArgs<TContentType> rulesFilterArgs = new RulesFilterArgs<TContentType>
            {
                ContentType = rule.ContentContainer.ContentType
            };

            IEnumerable<Rule<TContentType, TConditionType>> existentRules = await this.rulesDataSource.GetRulesByAsync(rulesFilterArgs).ConfigureAwait(false);

            List<string> errors = new List<string>();
            Rule<TContentType, TConditionType> existentRule = existentRules.FirstOrDefault(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
            if (existentRule is null)
            {
                errors.Add($"Rule with name '{rule.Name}' does not exist.");
            }

            if (errors.Any())
            {
                return RuleOperationResult.Error(errors);
            }

            int topPriorityThreshold = Math.Min(rule.Priority, existentRule.Priority);
            int bottomPriorityThreshold = Math.Max(rule.Priority, existentRule.Priority);

            switch (rule.Priority)
            {
                case int p when p > existentRule.Priority:
                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
                        .FilterPrioritiesRange(topPriorityThreshold, bottomPriorityThreshold)
                        .DecreasePriority()
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);
                    break;

                case int p when p < existentRule.Priority:
                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
                        .FilterPrioritiesRange(topPriorityThreshold, bottomPriorityThreshold)
                        .IncreasePriority()
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);
                    break;

                default:
                    await ManagementOperations.Manage(existentRules)
                        .UsingDataSource(this.rulesDataSource)
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