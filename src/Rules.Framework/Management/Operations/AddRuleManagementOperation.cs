namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Source;

    internal sealed class AddRuleManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly Rule<TContentType, TConditionType> rule;
        private readonly IRulesSource<TContentType, TConditionType> rulesDataSource;

        public AddRuleManagementOperation(IRulesSource<TContentType, TConditionType> rulesDataSource, Rule<TContentType, TConditionType> rule)
        {
            this.rulesDataSource = rulesDataSource;
            this.rule = rule;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            AddRuleArgs<TContentType, TConditionType> addRuleArgs = new()
            {
                Rule = this.rule,
            };

            await this.rulesDataSource.AddRuleAsync(addRuleArgs).ConfigureAwait(false);

            List<Rule<TContentType, TConditionType>> rulesResult = new List<Rule<TContentType, TConditionType>>(rules)
            {
                this.rule
            };

            return rulesResult;
        }
    }
}