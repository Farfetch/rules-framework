namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    /// <summary>
    /// Configurer for a rule's owner ruleset.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRuleConfigureRuleset<TRuleset, TCondition>
    {
        /// <summary>
        /// Sets the new rule to belong to the specified ruleset.
        /// </summary>
        /// <param name="ruleset">The ruleset.</param>
        /// <returns></returns>
        IRuleConfigureContent<TRuleset, TCondition> InRuleset(TRuleset ruleset);
    }
}