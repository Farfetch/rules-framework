namespace Rules.Framework.Builder
{
    /// <summary>
    /// The interface contract for a rules data source for rules with previously specified <typeparamref name="TContentType"/> and <typeparamref name="TConditionType"/>.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    public interface IRulesDataSourceSelector<TContentType, TConditionType>
    {
        /// <summary>
        /// Sets the rules data source for rules with previously specified <typeparamref name="TConditionType"/> and <typeparamref name="TContentType"/>.
        /// </summary>
        /// <param name="rulesDataSource">the rules data source.</param>
        /// <returns>a configured rules engine builder.</returns>
        IConfiguredRulesEngineBuilder<TContentType, TConditionType> SetDataSource(IRulesDataSource<TContentType, TConditionType> rulesDataSource);
    }
}