namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal sealed class AddRuleManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly Rule<TContentType, TConditionType> rule;
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;

        public AddRuleManagementOperation(IRulesDataSource<TContentType, TConditionType> rulesDataSource, Rule<TContentType, TConditionType> rule)
        {
            this.rulesDataSource = rulesDataSource;
            this.rule = rule;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            await this.rulesDataSource.AddRuleAsync(this.rule).ConfigureAwait(false);

            List<Rule<TContentType, TConditionType>> rulesResult = new List<Rule<TContentType, TConditionType>>(rules)
            {
                this.rule
            };

            return rulesResult;
        }
    }
}