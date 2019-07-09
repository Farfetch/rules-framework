namespace Rules.Framework
{
    using Rules.Framework.Builder;
    using static Rules.Framework.Builder.Selectors;

    /// <summary>
    /// Starts building a rules engine instance.
    /// </summary>
    public class RulesEngineBuilder
    {
        /// <summary>
        /// Starts building a rules engine.
        /// </summary>
        /// <returns>a content type selector.</returns>
        public IContentTypeSelector CreateRulesEngine() => new ContentTypeSelector();
    }
}