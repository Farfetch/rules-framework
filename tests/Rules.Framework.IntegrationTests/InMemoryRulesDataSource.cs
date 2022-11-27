namespace Rules.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal class InMemoryRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly List<Rule<TContentType, TConditionType>> rules;

        internal InMemoryRulesDataSource(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            this.rules = new List<Rule<TContentType, TConditionType>>(rules);
        }

        public Task AddRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            this.rules.Add(rule);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(
            TContentType contentType,
            DateTime dateBegin,
            DateTime dateEnd)
            => Task.FromResult<IEnumerable<Rule<TContentType, TConditionType>>>(
                this.rules.Where(r => object.Equals(r.ContentContainer.ContentType, contentType)).ToList());

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesByAsync(RulesFilterArgs<TContentType> rulesFilterArgs)
        {
            IEnumerable<Rule<TContentType, TConditionType>> result = this.rules.AsEnumerable();

            if (!object.Equals(rulesFilterArgs.ContentType, default(TContentType)))
            {
                result = result.Where(r => object.Equals(rulesFilterArgs.ContentType, r.ContentContainer.ContentType));
            }

            if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name))
            {
                result = result.Where(r => string.Equals(r.Name, rulesFilterArgs.Name));
            }

            if (rulesFilterArgs.Priority.HasValue)
            {
                result = result.Where(r => r.Priority == rulesFilterArgs.Priority.GetValueOrDefault());
            }

            return Task.FromResult<IEnumerable<Rule<TContentType, TConditionType>>>(result.Select(r => r.Clone()).ToList());
        }

        public Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            this.rules.RemoveAll(r => string.Equals(r.Name, rule.Name));

            this.rules.Add(rule);

            return Task.CompletedTask;
        }
    }
}