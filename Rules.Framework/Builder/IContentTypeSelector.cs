namespace Rules.Framework.Builder
{
    public interface IContentTypeSelector
    {
        IConditionTypeSelector<TContentType> WithContentType<TContentType>();
    }
}