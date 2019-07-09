namespace Rules.Framework
{
    /// <summary>
    /// Defines the rules engine behaviors when a condition is missing for rules evaluation.
    /// </summary>
    public enum MissingConditionBehaviors
    {
        /// <summary>
        /// When a condition is missing, instructs the rules engine to use the configured data type default for rule's condition.
        /// </summary>
        UseDataTypeDefault = 0,

        /// <summary>
        /// When a condition is missing, instructs the rules engine to discard the rule under evaluation (rule is considered not a match).
        /// </summary>
        Discard = 1
    }
}
