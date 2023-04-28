namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.SqlServer.Models;

    public class SqlServerProviderRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly IRuleFactory<TContentType, TConditionType> ruleFactory;

        private readonly IRulesFrameworkDbContext rulesFrameworkDbContext;

        public SqlServerProviderRulesDataSource(IRulesFrameworkDbContext rulesFrameworkDbContext,
            IRuleFactory<TContentType, TConditionType> ruleFactory)
        {
            //this.sqlConnection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));
            this.rulesFrameworkDbContext = rulesFrameworkDbContext;
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));
        }

        public Task AddRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            rulesFrameworkDbContext.Add(ruleDataModel);

            rulesFrameworkDbContext.SaveChanges();

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
                || (rule.DateBegin < dateBegin && (rule.DateEnd == null || rule.DateEnd > dateEnd))); // To fetch rules that begun before and end after filtered interval.

            return fetchedRules.ToList().Select(rule => this.ruleFactory.CreateRule(rule));
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

            return fetchedRules.Select(rule => this.ruleFactory.CreateRule(rule)).ToList();
        }

        public async Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            var ruleDataModel = this.ruleFactory.CreateRule(rule);

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

            //TODO: add update for each conditionNode (relations)
            //var conditionNode = rulesFrameworkDbContext.ConditionNodes.FirstOrDefault(cn => cn.Id == ruleDataModel.ConditionNode.Id);
            //if (conditionNode is object)
            //{
            //    conditionNode.DataTypeCode = ruleDataModel.ConditionNode.DataTypeCode;
            //    conditionNode.ConditionNodeTypeCode = ruleDataModel.ConditionNode.ConditionNodeTypeCode;
            //    conditionNode.ConditionTypeCode = ruleDataModel.ConditionNode.ConditionTypeCode;
            //    conditionNode.DataTypeCode = ruleDataModel.ConditionNode.DataTypeCode;
            //    conditionNode.Operand = ruleDataModel.ConditionNode.Operand;
            //    conditionNode.LogicalOperatorCode = ruleDataModel.ConditionNode.LogicalOperatorCode;

            //    foreach (var item in conditionNode.ConditionNodeRelations_ChildId.)
            //    {
            //    }
            //}

            await rulesFrameworkDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}