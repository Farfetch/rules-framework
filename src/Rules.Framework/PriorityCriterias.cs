namespace Rules.Framework
{
    /// <summary>
    /// Defines the available rules engine priority criterias to untie when multiple rules are
    /// matched to the set of conditions supplied.
    /// </summary>
    public enum PriorityCriterias
    {
        /// <summary>
        /// Sets the rule with the lowest priority number to win on a untie scenario.
        /// </summary>
        TopmostRuleWins = 0,

        /// <summary>
        /// Sets the rule with the highest priority number to win on a untie scenario.
        /// </summary>
        BottommostRuleWins = 1
    }
}