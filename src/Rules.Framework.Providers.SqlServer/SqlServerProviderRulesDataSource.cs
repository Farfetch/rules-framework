namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.SqlServer.Models;

    public class SqlServerProviderRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly IRuleFactory<TContentType, TConditionType> ruleFactory;

        private readonly RulesFrameworkDbContext rulesFrameworkDbContext;

        private readonly string sqlConnection;

        public SqlServerProviderRulesDataSource(string sqlConnection,
            IRuleFactory<TContentType, TConditionType> ruleFactory)
        {
            this.sqlConnection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));

            rulesFrameworkDbContext = new RulesFrameworkDbContext(); //TODO: how to override sql connection to use sqlConnection field? SqlConnectionStringBuilder
        }

        public Task AddRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            rulesFrameworkDbContext.Add(ruleDataModel);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            if (contentType is null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var fetchedRules = rulesFrameworkDbContext.Rules.Where(rule => rule.ContentTypeCode == Convert.ToInt32(contentType)); //todo: optimize

            fetchedRules.Where(rule =>
                   (rule.DateBegin >= dateBegin && rule.DateBegin < dateEnd)  // To fetch rules that begin during filtered interval but end after it.
                || (rule.DateEnd != null && rule.DateBegin >= dateBegin && rule.DateBegin < dateEnd) // To fetch rules that begun before filtered interval but end during it.
                || (rule.DateBegin < dateBegin || (rule.DateEnd == null && rule.DateEnd > dateEnd))); // To fetch rules that begun before and end after filtered interval.

            return fetchedRules.Select(rule => this.ruleFactory.CreateRule(rule));
        }

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesByAsync(RulesFilterArgs<TContentType> rulesFilterArgs) => throw new NotImplementedException();

        public Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule) => throw new NotImplementedException();
    }
}