namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal class UpdateRulesManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;

        public UpdateRulesManagementOperation(IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.rulesDataSource = rulesDataSource;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            foreach (Rule<TContentType, TConditionType> existentRule in rules)
            {
                await this.rulesDataSource.UpdateRuleAsync(existentRule).ConfigureAwait(false);
            }

            return rules;
        }
    }
}