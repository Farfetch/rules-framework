namespace Rules.Framework.Serialization
{
    /// <summary>
    /// Defines the interface contract for a content serialization provider. Provides content
    /// serializers per ruleset value, allowing for customization of serializers per each ruleset.
    /// </summary>
    public interface IContentSerializationProvider
    {
        /// <summary>
        /// Gets the content serializer associated with the given <paramref name="ruleset"/>.
        /// </summary>
        /// <param name="ruleset">the ruleset name.</param>
        /// <returns>the content serializer to deal with contents for the specified ruleset.</returns>
        IContentSerializer GetContentSerializer(string ruleset);
    }
}