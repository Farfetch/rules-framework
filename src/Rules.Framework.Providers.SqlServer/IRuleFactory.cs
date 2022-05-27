namespace Rules.Framework.Providers.SqlServer
{
    using Core = Rules.Framework.Core;
    using Model = Rules.Framework.SqlServer.Models;

    public interface IRuleFactory<TContentType, TConditionType>
    {
        Core.Rule<TContentType, TConditionType> CreateRule(Model.Rule ruleDataModel);

        Model.Rule CreateRule(Core.Rule<TContentType, TConditionType> rule);
    }
}