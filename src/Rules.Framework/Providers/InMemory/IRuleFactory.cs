namespace Rules.Framework.Providers.InMemory
{
    using Rules.Framework.Providers.InMemory.DataModel;

    internal interface IRuleFactory
    {
        Rule CreateRule(RuleDataModel ruleDataModel);

        RuleDataModel CreateRule(Rule rule);
    }
}