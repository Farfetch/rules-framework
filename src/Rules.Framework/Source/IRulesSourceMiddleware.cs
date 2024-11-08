namespace Rules.Framework.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IRulesSourceMiddleware
    {
        Task HandleAddRuleAsync(
            AddRuleArgs args,
            AddRuleDelegate next);

        Task HandleCreateRulesetAsync(
            CreateRulesetArgs args,
            CreateRulesetDelegate next);

        Task<IEnumerable<Rule>> HandleGetRulesAsync(
            GetRulesArgs args,
            GetRulesDelegate next);

        Task<IEnumerable<Ruleset>> HandleGetRulesetsAsync(
            GetRulesetsArgs args,
            GetRulesetsDelegate next);

        Task<IEnumerable<Rule>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs args,
            GetRulesFilteredDelegate next);

        Task HandleUpdateRuleAsync(
            UpdateRuleArgs args,
            UpdateRuleDelegate next);
    }
}