using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rules.Framework.Core;

namespace Rules.Framework.IntegrationTests
{
    internal class InMemoryRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly IEnumerable<Rule<TContentType, TConditionType>> rules;

        internal InMemoryRulesDataSource(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            this.rules = rules;
        }

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(
            TContentType contentType,
            DateTime dateBegin,
            DateTime dateEnd)
            => Task.FromResult(this.rules.Where(r => object.Equals(r.ContentContainer.ContentType, contentType)));
    }
}