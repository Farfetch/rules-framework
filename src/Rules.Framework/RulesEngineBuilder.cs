namespace Rules.Framework
{
    using Rules.Framework.Builder;
    using static Rules.Framework.Builder.RulesEngineSelectors;

    /// <summary>
    /// Starts building a rules engine instance.
    /// </summary>
    public static class RulesEngineBuilder
    {
        /// <summary>
        /// Starts building a rules engine.
        /// </summary>
        /// <returns>the rules data source selector.</returns>
        public static IRulesDataSourceSelector CreateRulesEngine() => new RulesDataSourceSelector();
    }
}