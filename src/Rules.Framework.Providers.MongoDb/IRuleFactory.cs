namespace Rules.Framework.Providers.MongoDb
{
    using Rules.Framework.Core;
    using Rules.Framework.Providers.MongoDb.DataModel;

    internal interface IRuleFactory<TContentType, TConditionType>
    {
        Rule<TContentType, TConditionType> CreateRule(RuleDataModel ruleDataModel);

        RuleDataModel CreateRule(Rule<TContentType, TConditionType> rule);
    }
}