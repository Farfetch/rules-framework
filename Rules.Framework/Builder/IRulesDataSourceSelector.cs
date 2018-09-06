namespace Rules.Framework.Builder
{
    public interface IRulesDataSourceSelector<TContentType, TConditionType>
    {
        RulesEngine<TContentType, TConditionType> SetDataSource(IRulesDataSource<TContentType, TConditionType> rulesDataSource);
    }
}