namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Source;

    internal sealed class UpdateRulesManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;

        public UpdateRulesManagementOperation(IRulesSource<TContentType, TConditionType> rulesSource)
        {
            this.rulesSource = rulesSource;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            foreach (Rule<TContentType, TConditionType> existentRule in rules)
            {
                UpdateRuleArgs<TContentType, TConditionType> updateRuleArgs = new()
                {
                    Rule = existentRule,
                };

                await this.rulesSource.UpdateRuleAsync(updateRuleArgs).ConfigureAwait(false);
            }

            return rules;
        }
    }
}