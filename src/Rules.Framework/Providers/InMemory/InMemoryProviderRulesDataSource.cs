namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory.DataModel;

    /// <summary>
    /// The rules data source implementation for usage backed with a in-memory database.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    /// <seealso cref="Rules.Framework.IRulesDataSource{TContentType, TConditionType}"/>
    public class InMemoryProviderRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly IInMemoryRulesStorage<TContentType, TConditionType> inMemoryRulesStorage;
        private readonly IRuleFactory<TContentType, TConditionType> ruleFactory;

        internal InMemoryProviderRulesDataSource(
            IInMemoryRulesStorage<TContentType, TConditionType> inMemoryRulesStorage,
            IRuleFactory<TContentType, TConditionType> ruleFactory)
        {
            this.inMemoryRulesStorage = inMemoryRulesStorage ?? throw new ArgumentNullException(nameof(inMemoryRulesStorage));
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));
        }

        /// <summary>
        /// Adds a new rule to data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public Task AddRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return Task.Run(() =>
            {
                RuleDataModel<TContentType, TConditionType> ruleDataModel = this.ruleFactory.CreateRule(rule);

                this.inMemoryRulesStorage.AddRule(ruleDataModel);
            });
        }

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="contentType"/> between
        /// <paramref name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="contentType">the content type categorization.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            return Task.Run(() =>
            {
                var filteredByContent = this.inMemoryRulesStorage.GetRulesBy(contentType);

                var filteredByDate = filteredByContent.Where(rule =>
                    rule.Active &&
                    rule.DateBegin <= dateEnd
                    && (rule.DateEnd is null || rule.DateEnd > dateBegin)
                );

                return filteredByDate.Select(r => this.ruleFactory.CreateRule(r)).AsEnumerable();
            });
        }

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesFilterArgs</exception>
        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesByAsync(RulesFilterArgs<TContentType> rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            return Task.Run(() =>
            {
                IEnumerable<RuleDataModel<TContentType, TConditionType>> ruleDataModels = this.inMemoryRulesStorage.GetAllRules();

                IEnumerable<RuleDataModel<TContentType, TConditionType>> filtered = ruleDataModels;

                if (!object.Equals(rulesFilterArgs.ContentType, default(TContentType)))
                {
                    filtered = filtered.Where(r => object.Equals(r.ContentType, rulesFilterArgs.ContentType));
                }

                if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name))
                {
                    filtered = filtered.Where(r => string.Equals(r.Name, rulesFilterArgs.Name, StringComparison.InvariantCultureIgnoreCase));
                }

                if (rulesFilterArgs.Priority.HasValue)
                {
                    filtered = filtered.Where(r => r.Priority == rulesFilterArgs.Priority);
                }

                return filtered.Select(r => this.ruleFactory.CreateRule(r)).AsEnumerable();
            });
        }

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return Task.Run(() =>
            {
                RuleDataModel<TContentType, TConditionType> newRuleDataModel = this.ruleFactory.CreateRule(rule);

                this.inMemoryRulesStorage.UpdateRule(newRuleDataModel);
            });
        }
    }
}