namespace Rules.Framework.Builder
{
    /// <summary>
    /// The interface contract for a rules data source for rules.
    /// </summary>
    public interface IRulesDataSourceSelector
    {
        /// <summary>
        /// Sets the rules data source for rules.
        /// </summary>
        /// <param name="rulesDataSource">the rules data source.</param>
        /// <returns>a configured rules engine builder.</returns>
        IConfiguredRulesEngineBuilder SetDataSource(IRulesDataSource rulesDataSource);
    }
}