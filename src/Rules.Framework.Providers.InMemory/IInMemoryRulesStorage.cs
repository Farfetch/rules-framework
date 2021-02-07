namespace Rules.Framework.Providers.InMemory
{
    using System.Collections.Generic;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal interface IInMemoryRulesStorage<TContentType, TConditionType>
    {
        void AddRule(RuleDataModel<TContentType, TConditionType> ruleDataModel);

        IEnumerable<RuleDataModel<TContentType, TConditionType>> GetAllRules();

        IEnumerable<RuleDataModel<TContentType, TConditionType>> GetRulesBy(TContentType contentType);

        void UpdateRule(RuleDataModel<TContentType, TConditionType> ruleDataModel);
    }
}