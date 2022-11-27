namespace Rules.Framework.Source
{
    using System.Threading.Tasks;

    internal delegate Task AddRuleDelegate<TContentType, TConditionType>(
        AddRuleArgs<TContentType, TConditionType> args);
}
