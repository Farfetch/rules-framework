namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRulesEngine<TContentType, TConditionType>
    {
        IRulesEngineOptions Options { get; }

        Task<RuleOperationResult> ActivateRuleAsync(Rule<TContentType, TConditionType> rule);

        Task<RuleOperationResult> AddRuleAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption);

        Task<RuleOperationResult> DeactivateRuleAsync(Rule<TContentType, TConditionType> rule);

        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        Task<IEnumerable<TContentType>> GetContentTypesAsync();

        Task<IEnumerable<TConditionType>> GetUniqueConditionTypesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd);

        Task<IEnumerable<Rule<TContentType, TConditionType>>> MatchManyAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions);

        Task<Rule<TContentType, TConditionType>> MatchOneAsync(TContentType contentType, DateTime matchDateTime, IEnumerable<Condition<TConditionType>> conditions);

        Task<IEnumerable<Rule<TContentType, TConditionType>>> SearchAsync(SearchArgs<TContentType, TConditionType> searchArgs);

        Task<RuleOperationResult> UpdateRuleAsync(Rule<TContentType, TConditionType> rule);
    }
}