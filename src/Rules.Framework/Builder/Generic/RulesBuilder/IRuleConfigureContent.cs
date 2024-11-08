namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    using Rules.Framework.Serialization;

    /// <summary>
    /// Configurer for a rule's content.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRuleConfigureContent<TRuleset, TCondition>
    {
        /// <summary>
        /// Sets the new rule with the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        IRuleConfigureDateBegin<TRuleset, TCondition> SetContent(object content);

        /// <summary>
        /// Sets the new rule with the specified serialized content.
        /// </summary>
        /// <param name="content">The serialized content.</param>
        /// <param name="contentSerializationProvider">The content serialization provider.</param>
        /// <returns></returns>
        IRuleConfigureDateBegin<TRuleset, TCondition> SetContent(object content, IContentSerializationProvider contentSerializationProvider);
    }
}