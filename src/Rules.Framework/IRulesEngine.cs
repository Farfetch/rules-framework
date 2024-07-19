namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRulesEngine
    {
        IRulesEngineOptions Options { get; }

        Task<RuleOperationResult> ActivateRuleAsync(Rule rule);

        Task<RuleOperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption);

        Task<RuleOperationResult> DeactivateRuleAsync(Rule rule);

        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        Task<IEnumerable<string>> GetContentTypesAsync();

        Task<IEnumerable<string>> GetUniqueConditionTypesAsync(string contentType, DateTime dateBegin, DateTime dateEnd);

        Task<IEnumerable<Rule>> MatchManyAsync(string contentType, DateTime matchDateTime, IEnumerable<Condition<string>> conditions);

        Task<Rule> MatchOneAsync(string contentType, DateTime matchDateTime, IEnumerable<Condition<string>> conditions);

        Task<IEnumerable<Rule>> SearchAsync(SearchArgs<string, string> searchArgs);

        Task<RuleOperationResult> UpdateRuleAsync(Rule rule);
    }
}