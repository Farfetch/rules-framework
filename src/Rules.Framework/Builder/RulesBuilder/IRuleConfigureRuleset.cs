namespace Rules.Framework.Builder.RulesBuilder
{
    /// <summary>
    /// Configurer for a rule's owner ruleset.
    /// </summary>
    public interface IRuleConfigureRuleset
    {
        /// <summary>
        /// Sets the new rule to belong to the specified ruleset.
        /// </summary>
        /// <param name="ruleset">The ruleset.</param>
        /// <returns></returns>
        IRuleConfigureContent InRuleset(string ruleset);
    }
}