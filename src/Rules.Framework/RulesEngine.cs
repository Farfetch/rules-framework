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
    using Rules.Framework.Evaluation;
    using Rules.Framework.Extensions;
    using Rules.Framework.Management;
    using Rules.Framework.Source;
    using Rules.Framework.Validation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    public class RulesEngine : IRulesEngine
    {
        private readonly IConditionsEvalEngine conditionsEvalEngine;
        private readonly IConditionTypeExtractor conditionTypeExtractor;
        private readonly IRulesSource rulesSource;
        private readonly RuleValidator ruleValidator = RuleValidator.Instance;
        private readonly IValidatorProvider validatorProvider;

        internal RulesEngine(
            IConditionsEvalEngine conditionsEvalEngine,
            IRulesSource rulesSource,
            IValidatorProvider validatorProvider,
            RulesEngineOptions rulesEngineOptions,
            IConditionTypeExtractor conditionTypeExtractor)
        {
            this.conditionsEvalEngine = conditionsEvalEngine;
            this.rulesSource = rulesSource;
            this.validatorProvider = validatorProvider;
            this.Options = rulesEngineOptions;
            this.conditionTypeExtractor = conditionTypeExtractor;
        }

        /// <inheritdoc/>
        public IRulesEngineOptions Options { get; }

        /// <inheritdoc/>
        public Task<OperationResult> ActivateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            rule.Active = true;

            return this.UpdateRuleInternalAsync(rule);
        }

        /// <inheritdoc/>
        public Task<OperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (ruleAddPriorityOption is null)
            {
                throw new ArgumentNullException(nameof(ruleAddPriorityOption));
            }

            return this.AddRuleInternalAsync(rule, ruleAddPriorityOption);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> CreateContentTypeAsync(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var getContentTypesArgs = new GetContentTypesArgs();
            var existentContentTypes = await this.rulesSource.GetContentTypesAsync(getContentTypesArgs).ConfigureAwait(false);
            if (existentContentTypes.Contains(contentType, StringComparer.Ordinal))
            {
                return OperationResult.Error($"The content type '{contentType}' already exists.");
            }

            return await this.CreateContentTypeInternalAsync(contentType).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<OperationResult> DeactivateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            rule.Active = false;

            return this.UpdateRuleInternalAsync(rule);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> GetContentTypesAsync()
        {
            return this.rulesSource.GetContentTypesAsync(new GetContentTypesArgs());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetUniqueConditionTypesAsync(string contentType, DateTime dateBegin, DateTime dateEnd)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = contentType,
                DateBegin = dateBegin,
                DateEnd = dateEnd,
            };

            var matchedRules = await this.rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

            return this.conditionTypeExtractor.GetConditionTypes(matchedRules);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rule>> MatchManyAsync(
            string contentType,
            DateTime matchDateTime,
            IEnumerable<Condition<string>> conditions)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = contentType,
                DateBegin = matchDateTime,
                DateEnd = matchDateTime,
            };

            var conditionsAsDictionary = conditions.ToDictionary(ks => ks.Type, ks => ks.Value, StringComparer.Ordinal);
            var orderedRules = await this.GetRulesOrderedAscendingAsync(getRulesArgs).ConfigureAwait(false);
            return this.EvalAll(orderedRules, evaluationOptions, conditionsAsDictionary, active: true);
        }

        /// <inheritdoc/>
        public async Task<Rule> MatchOneAsync(
            string contentType,
            DateTime matchDateTime,
            IEnumerable<Condition<string>> conditions)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = contentType,
                DateBegin = matchDateTime,
                DateEnd = matchDateTime,
            };

            var conditionsAsDictionary = conditions.ToDictionary(ks => ks.Type, ks => ks.Value);
            var orderedRules = await this.GetRulesOrderedAscendingAsync(getRulesArgs).ConfigureAwait(false);
            return this.Options.PriorityCriteria == PriorityCriterias.TopmostRuleWins
                ? EvalOneTraverse(orderedRules, evaluationOptions, conditionsAsDictionary, active: true)
                : EvalOneReverse(orderedRules, evaluationOptions, conditionsAsDictionary, active: true);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Rule>> SearchAsync(SearchArgs<string, string> searchArgs)
        {
            if (searchArgs is null)
            {
                throw new ArgumentNullException(nameof(searchArgs));
            }

            var validator = this.validatorProvider.GetValidatorFor<SearchArgs<string, string>>();
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

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = searchArgs.ContentType,
                DateBegin = searchArgs.DateBegin,
                DateEnd = searchArgs.DateEnd,
            };

            var conditionsAsDictionary = searchArgs.Conditions.ToDictionary(ks => ks.Type, ks => ks.Value);
            var orderedRules = await this.GetRulesOrderedAscendingAsync(getRulesArgs).ConfigureAwait(false);
            return this.EvalAll(orderedRules, evaluationOptions, conditionsAsDictionary, searchArgs.Active);
        }

        /// <inheritdoc/>
        public Task<OperationResult> UpdateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return this.UpdateRuleInternalAsync(rule);
        }

        private async Task<OperationResult> AddRuleInternalAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var errors = new List<string>();
            var contentTypes = await this.rulesSource.GetContentTypesAsync(new GetContentTypesArgs()).ConfigureAwait(false);

            if (!contentTypes.Contains(rule.ContentType, StringComparer.Ordinal))
            {
                if (!this.Options.AutoCreateContentTypes)
                {
                    errors.Add($"Specified content type '{rule.ContentType}' does not exist. " +
                        $"Please create the content type first or set the rules engine option '{nameof(this.Options.AutoCreateContentTypes)}' to true.");
                    return OperationResult.Error(errors);
                }

                await this.CreateContentTypeInternalAsync(rule.ContentType).ConfigureAwait(false);
            }

            var rulesFilterArgs = new GetRulesFilteredArgs
            {
                ContentType = rule.ContentType,
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
                return OperationResult.Error(errors);
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

            return OperationResult.Success();
        }

        private async Task AddRuleInternalAtBottomAsync(Rule rule, IEnumerable<Rule> existentRules)
        {
            rule.Priority = !existentRules.Any() ? 1 : existentRules.Max(r => r.Priority) + 1;

            await ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .AddRule(rule)
                .ExecuteOperationsAsync()
                .ConfigureAwait(false);
        }

        private async Task AddRuleInternalAtPriorityNumberAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption, IEnumerable<Rule> existentRules)
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

        private async Task AddRuleInternalAtRuleNameAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption, IEnumerable<Rule> existentRules)
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

        private async Task AddRuleInternalAtTopAsync(Rule rule, IEnumerable<Rule> existentRules)
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

        private async Task<OperationResult> CreateContentTypeInternalAsync(string contentType)
        {
            var createContentTypeArgs = new CreateContentTypeArgs { Name = contentType };
            await this.rulesSource.CreateContentTypeAsync(createContentTypeArgs).ConfigureAwait(false);
            return OperationResult.Success();
        }

        private IEnumerable<Rule> EvalAll(
            List<Rule> orderedRules,
            EvaluationOptions evaluationOptions,
            Dictionary<string, object> conditionsAsDictionary,
            bool? active)
        {
            // Begins evaluation at the first element of the given list as parameter. Returns all
            // rules that match. Assumes given list is ordered.
            var matchedRules = new List<Rule>(orderedRules.Count);
            foreach (var rule in orderedRules)
            {
                if (this.EvalRule(rule, evaluationOptions, conditionsAsDictionary, active))
                {
                    matchedRules.Add(rule);
                }
            }

            return matchedRules.AsReadOnly();
        }

        private Rule EvalOneReverse(
            List<Rule> rules,
            EvaluationOptions evaluationOptions,
            Dictionary<string, object> conditionsAsDictionary,
            bool? active)
        {
            // Begins evaluation at the last element of the given list as parameter. Returns the
            // first rule that matches. Assumes given list is ordered.
            for (var i = rules.Count - 1; i >= 0; i--)
            {
                var rule = rules[i];
                if (this.EvalRule(rule, evaluationOptions, conditionsAsDictionary, active))
                {
                    return rule;
                }
            }

            return null!;
        }

        private Rule EvalOneTraverse(
            List<Rule> rules,
            EvaluationOptions evaluationOptions,
            Dictionary<string, object> conditionsAsDictionary,
            bool? active)
        {
            // Begins evaluation at the first element of the given list as parameter. Returns the
            // first rule that matches. Assumes given list is ordered.
            for (var i = 0; i < rules.Count; i++)
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
            Rule rule,
            EvaluationOptions evaluationOptions,
            Dictionary<string, object> conditionsAsDictionary,
            bool? active)
            => rule.Active == active.GetValueOrDefault(defaultValue: true) && (rule.RootCondition == null || this.conditionsEvalEngine.Eval(rule.RootCondition, conditionsAsDictionary, evaluationOptions));

        private async Task<List<Rule>> GetRulesOrderedAscendingAsync(GetRulesArgs getRulesArgs)
        {
            var rules = await this.rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);
            var orderedRules = new List<Rule>(rules.Count());
            var greatestPriority = 0;
            foreach (var rule in rules)
            {
                if (orderedRules.Count == 0 || rule.Priority > greatestPriority)
                {
                    orderedRules.Add(rule);
                    greatestPriority = rule.Priority;
                    continue;
                }

                for (var i = 0; i < orderedRules.Count; i++)
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

        private async Task<OperationResult> UpdateRuleInternalAsync(Rule rule)
        {
            var rulesFilterArgs = new GetRulesFilteredArgs
            {
                ContentType = rule.ContentType,
            };

            var existentRules = await this.rulesSource.GetRulesFilteredAsync(rulesFilterArgs).ConfigureAwait(false);

            var existentRule = existentRules.FirstOrDefault(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
            if (existentRule is null)
            {
                return OperationResult.Error($"Rule with name '{rule.Name}' does not exist.");
            }

            var validationResult = this.ruleValidator.Validate(rule);

            if (!validationResult.IsValid)
            {
                return OperationResult.Error(validationResult.Errors.Select(ve => ve.ErrorMessage));
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

            return OperationResult.Success();
        }
    }
}