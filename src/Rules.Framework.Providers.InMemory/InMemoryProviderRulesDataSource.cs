namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory.DataModel;

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

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            return Task.Run(() =>
            {
                IEnumerable<RuleDataModel<TContentType, TConditionType>> ruleDataModels = this.inMemoryRulesStorage.GetRulesBy(contentType);

                IEnumerable<RuleDataModel<TContentType, TConditionType>> filteredByDate = ruleDataModels.Where(r =>
                    (r.DateBegin >= dateBegin && r.DateBegin < dateEnd)
                    || (r.DateEnd is null && r.DateEnd >= dateBegin && r.DateEnd < dateEnd)
                    || (r.DateBegin < dateBegin && (r.DateEnd is null || r.DateEnd > dateEnd)));

                return filteredByDate.Select(r => this.ruleFactory.CreateRule(r)).ToList().AsEnumerable();
            });
        }

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

                return filtered.Select(r => this.ruleFactory.CreateRule(r)).ToList().AsEnumerable();
            });
        }

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