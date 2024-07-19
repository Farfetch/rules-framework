namespace Rules.Framework.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IRulesSource
    {
        Task AddRuleAsync(AddRuleArgs args);

        Task CreateRulesetAsync(CreateRulesetArgs args);

        Task<IEnumerable<Rule>> GetRulesAsync(GetRulesArgs args);

        Task<IEnumerable<Ruleset>> GetRulesetsAsync(GetRulesetsArgs args);

        Task<IEnumerable<Rule>> GetRulesFilteredAsync(GetRulesFilteredArgs args);

        Task UpdateRuleAsync(UpdateRuleArgs args);
    }
}