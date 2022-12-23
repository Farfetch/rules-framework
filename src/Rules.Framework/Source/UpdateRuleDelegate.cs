namespace Rules.Framework.Source
{
    using System.Threading.Tasks;

    internal delegate Task UpdateRuleDelegate<TContentType, TConditionType>(
        UpdateRuleArgs<TContentType, TConditionType> args);
}
