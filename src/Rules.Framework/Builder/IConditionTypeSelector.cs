namespace Rules.Framework.Builder
{
    public interface IConditionTypeSelector<TContentType>
    {
        IRulesDataSourceSelector<TContentType, TConditionType> WithConditionType<TConditionType>();
    }
}