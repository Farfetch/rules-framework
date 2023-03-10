namespace Rules.Framework.Providers.InMemory
{
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal interface IRuleFactory<TContentType, TConditionType>
    {
        Rule<TContentType, TConditionType> CreateRule(RuleDataModel<TContentType, TConditionType> ruleDataModel);

        RuleDataModel<TContentType, TConditionType> CreateRule(Rule<TContentType, TConditionType> rule);
    }
}