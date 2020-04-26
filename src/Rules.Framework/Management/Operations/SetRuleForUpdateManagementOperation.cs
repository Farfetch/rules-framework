namespace Rules.Framework.Management.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal class SetRuleForUpdateManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly Rule<TContentType, TConditionType> updatedRule;

        public SetRuleForUpdateManagementOperation(Rule<TContentType, TConditionType> updatedRule)
        {
            this.updatedRule = updatedRule;
        }

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            List<Rule<TContentType, TConditionType>> result = new List<Rule<TContentType, TConditionType>>(rules);

            result.RemoveAll(r => string.Equals(r.Name, this.updatedRule.Name, StringComparison.InvariantCultureIgnoreCase));
            result.Add(this.updatedRule);

            return Task.FromResult(result.AsEnumerable());
        }
    }
}