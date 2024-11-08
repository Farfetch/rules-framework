namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The rules data source implementation for usage backed with a in-memory database.
    /// </summary>
    /// <seealso cref="Rules.Framework.IRulesDataSource"/>
    public class InMemoryProviderRulesDataSource : IRulesDataSource
    {
        private readonly IInMemoryRulesStorage inMemoryRulesStorage;
        private readonly IRuleFactory ruleFactory;

        internal InMemoryProviderRulesDataSource(
            IInMemoryRulesStorage inMemoryRulesStorage,
            IRuleFactory ruleFactory)
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
        public Task AddRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            this.inMemoryRulesStorage.AddRule(ruleDataModel);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new ruleset on the data source.
        /// </summary>
        /// <param name="ruleset">the ruleset name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">ruleset</exception>
        /// <exception cref="System.InvalidOperationException">
        /// The ruleset '{ruleset}' already exists.
        /// </exception>
        public Task CreateRulesetAsync(string ruleset)
        {
            if (string.IsNullOrWhiteSpace(ruleset))
            {
                throw new ArgumentNullException(nameof(ruleset));
            }

            var rulesets = this.inMemoryRulesStorage.GetRulesets();

            if (rulesets.Any(rs => string.Equals(rs.Name, ruleset, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"The ruleset '{ruleset}' already exists.");
            }

            this.inMemoryRulesStorage.CreateRuleset(ruleset);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="ruleset"/> between <paramref
        /// name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="ruleset">the ruleset.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        public Task<IEnumerable<Rule>> GetRulesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var filteredByRuleset = this.inMemoryRulesStorage.GetRulesBy(ruleset);

            var filteredRules = new Rule[filteredByRuleset.Count];
            var i = 0;
            foreach (var ruleDataModel in filteredByRuleset)
            {
                if (ruleDataModel.DateBegin <= dateEnd && (ruleDataModel.DateEnd is null || ruleDataModel.DateEnd > dateBegin))
                {
                    var rule = this.ruleFactory.CreateRule(ruleDataModel);
                    filteredRules[i++] = rule;
                }
            }

            if (filteredRules.Length > i)
            {
                Array.Resize(ref filteredRules, i);
            }

            return Task.FromResult<IEnumerable<Rule>>(filteredRules);
        }

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesFilterArgs</exception>
        public Task<IEnumerable<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            var ruleDataModels = this.inMemoryRulesStorage.GetAllRules();

            var filteredRules = new Rule[ruleDataModels.Count];
            var i = 0;
            foreach (var ruleDataModel in ruleDataModels)
            {
                if (!object.Equals(rulesFilterArgs.Ruleset, default(string))
                    && !object.Equals(ruleDataModel.Ruleset, rulesFilterArgs.Ruleset))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name)
                    && !string.Equals(ruleDataModel.Name, rulesFilterArgs.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (rulesFilterArgs.Priority.HasValue
                    && ruleDataModel.Priority == rulesFilterArgs.Priority)
                {
                    continue;
                }

                var rule = this.ruleFactory.CreateRule(ruleDataModel);
                filteredRules[i++] = rule;
            }

            if (filteredRules.Length > i)
            {
                Array.Resize(ref filteredRules, i);
            }

            return Task.FromResult<IEnumerable<Rule>>(filteredRules);
        }

        /// <summary>
        /// Gets the rulesets from the data source.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Ruleset>> GetRulesetsAsync()
        {
            var rulesetDataModels = this.inMemoryRulesStorage.GetRulesets();
            var rulesets = new Ruleset[rulesetDataModels.Count];
            var i = 0;
            foreach (var rulesetDataModel in rulesetDataModels)
            {
                rulesets[i++] = new Ruleset(rulesetDataModel.Name, rulesetDataModel.Creation);
            }

            return Task.FromResult<IEnumerable<Ruleset>>(rulesets);
        }

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public Task UpdateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var newRuleDataModel = this.ruleFactory.CreateRule(rule);

            this.inMemoryRulesStorage.UpdateRule(newRuleDataModel);

            return Task.CompletedTask;
        }
    }
}