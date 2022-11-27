namespace Rules.Framework.Source
{
    using Rules.Framework.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IRulesSource<TContentType, TConditionType>
    {
        Task AddRuleAsync(AddRuleArgs<TContentType, TConditionType> args);

        Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(GetRulesArgs<TContentType> args);

        Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesFilteredAsync(GetRulesFilteredArgs<TContentType> args);

        Task UpdateRuleAsync(UpdateRuleArgs<TContentType, TConditionType> args);
    }
}
