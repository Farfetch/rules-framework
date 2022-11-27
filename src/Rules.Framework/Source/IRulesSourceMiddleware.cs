namespace Rules.Framework.Source
{
    using Rules.Framework.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IRulesSourceMiddleware<TContentType, TConditionType>
    {
        Task HandleAddRuleAsync(
            AddRuleArgs<TContentType, TConditionType> args,
            AddRuleDelegate<TContentType, TConditionType> next);

        Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesAsync(
            GetRulesArgs<TContentType> args,
            GetRulesDelegate<TContentType, TConditionType> next);

        Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs<TContentType> args,
            GetRulesFilteredDelegate<TContentType, TConditionType> next);

        Task HandleUpdateRuleAsync(
            UpdateRuleArgs<TContentType, TConditionType> args,
            UpdateRuleDelegate<TContentType, TConditionType> next);
    }
}
