namespace Rules.Framework.Builder
{
    /// <summary>
    /// Exposes the interface contract for selecting a rules engine content type.
    /// </summary>
    public interface IContentTypeSelector
    {
        /// <summary>
        /// Sets the rules engine content type to use, according to supplied <typeparamref name="TContentType"/>.
        /// </summary>
        /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
        /// <returns>a condition type selector.</returns>
        IConditionTypeSelector<TContentType> WithContentType<TContentType>();
    }
}