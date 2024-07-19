namespace Rules.Framework.Providers.MongoDb
{
    using Rules.Framework.Providers.MongoDb.DataModel;

    internal interface IRuleFactory
    {
        Rule CreateRule(RuleDataModel ruleDataModel);

        RuleDataModel CreateRule(Rule rule);
    }
}