namespace Rules.Framework.Builder.RulesBuilder
{
    using Rules.Framework.Serialization;

    /// <summary>
    /// Configurer for a rule's content.
    /// </summary>
    public interface IRuleConfigureContent
    {
        /// <summary>
        /// Sets the new rule with the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        IRuleConfigureDateBegin SetContent(object content);

        /// <summary>
        /// Sets the new rule with the specified serialized content.
        /// </summary>
        /// <param name="content">The serialized content.</param>
        /// <param name="contentSerializationProvider">The content serialization provider.</param>
        /// <returns></returns>
        IRuleConfigureDateBegin SetContent(object content, IContentSerializationProvider contentSerializationProvider);
    }
}