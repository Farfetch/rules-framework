namespace Rules.Framework.Builder
{
    public interface IRulesDataSourceSelector<TContentType, TConditionType>
    {
        IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetDataSource(IRulesDataSource<TContentType, TConditionType> rulesDataSource);
    }
}