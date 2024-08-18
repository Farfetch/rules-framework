namespace Rules.Framework.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IRulesSource
    {
        Task AddRuleAsync(AddRuleArgs args);

        Task CreateContentTypeAsync(CreateContentTypeArgs args);

        Task<IEnumerable<string>> GetContentTypesAsync(GetContentTypesArgs args);

        Task<IEnumerable<Rule>> GetRulesAsync(GetRulesArgs args);

        Task<IEnumerable<Rule>> GetRulesFilteredAsync(GetRulesFilteredArgs args);

        Task UpdateRuleAsync(UpdateRuleArgs args);
    }
}