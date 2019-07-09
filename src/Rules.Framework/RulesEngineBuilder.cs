using Rules.Framework.Builder;
using static Rules.Framework.Builder.Selectors;

namespace Rules.Framework
{
    public class RulesEngineBuilder
    {
        public IContentTypeSelector CreateRulesEngine() => new ContentTypeSelector();
    }
}