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

            fetchedRules = fetchedRules.Where(rule =>
                   (rule.DateBegin >= dateBegin && rule.DateBegin < dateEnd)  // To fetch rules that begin during filtered interval but end after it.
                || (rule.DateEnd != null && rule.DateBegin >= dateBegin && rule.DateBegin < dateEnd) // To fetch rules that begun before filtered interval but end during it.
                || (rule.DateBegin < dateBegin || (rule.DateEnd == null && rule.DateEnd > dateEnd))); // To fetch rules that begun before and end after filtered interval.

            return fetchedRules.Select(rule => this.ruleFactory.CreateRule(rule));
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesByAsync(RulesFilterArgs<TContentType> rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            var fetchedRules = rulesFrameworkDbContext.Rules.AsQueryable();

            if (!EqualityComparer<TContentType>.Default.Equals(rulesFilterArgs.ContentType, default(TContentType)))
            {
                fetchedRules = fetchedRules.Where(rule => rule.ContentTypeCode == Convert.ToInt32(rulesFilterArgs.ContentType)); //todo: optimize
            }

            if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name))
            {
                fetchedRules = fetchedRules.Where(rule => rule.Name == rulesFilterArgs.Name);
            }
            if (rulesFilterArgs.Priority.HasValue)
            {
                fetchedRules = fetchedRules.Where(rule => rule.Priority == rulesFilterArgs.Priority.Value);
            }

            foreach (var rule in fetchedRules)
            {
                await rulesFrameworkDbContext.Entry(rule).Reference(r => r.ConditionNode).LoadAsync(); // TODO: Evaluate performance
                //await rulesFrameworkDbContext.Entry(rule).Reference(r => r.ConditionNode.DataType).LoadAsync(); // TODO: Evaluate performance
                //await rulesFrameworkDbContext.Entry(rule).Reference(r => r.ConditionNode.ConditionType).LoadAsync(); // TODO: Evaluate performance
                //await rulesFrameworkDbContext.Entry(rule).Reference(r => r.ConditionNode.LogicalOperator).LoadAsync(); // TODO: Evaluate performance
                //await rulesFrameworkDbContext.Entry(rule).Reference(r => r.ConditionNode.Operator).LoadAsync(); // TODO: Evaluate performance
                //await rulesFrameworkDbContext.Entry(rule).Reference(r => r.ConditionNode.ConditionNodeType).LoadAsync(); // TODO: Evaluate performance
            }

            return fetchedRules.Select(rule => this.ruleFactory.CreateRule(rule));
        }

        public async Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            Rule ruleDataModel = this.ruleFactory.CreateRule(rule);

            var fetchedRule = rulesFrameworkDbContext.Rules.FirstOrDefault(r => r.Name == rule.Name);

            if (fetchedRule is null)
            {
                return;
            }

            fetchedRule.Content = ruleDataModel.Content;
            fetchedRule.ContentTypeCode = ruleDataModel.ContentTypeCode;
            fetchedRule.DateBegin = ruleDataModel.DateBegin;
            fetchedRule.DateEnd = ruleDataModel.DateEnd;
            fetchedRule.Priority = ruleDataModel.Priority;
            fetchedRule.ConditionNodeId = ruleDataModel.ConditionNodeId;

            await rulesFrameworkDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}