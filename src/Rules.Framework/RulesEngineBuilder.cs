namespace Rules.Framework
{
    using Rules.Framework.Builder;
    using static Rules.Framework.Builder.Selectors;

    public class RulesEngineBuilder
    {
        public IContentTypeSelector CreateRulesEngine() => new ContentTypeSelector();
    }
}