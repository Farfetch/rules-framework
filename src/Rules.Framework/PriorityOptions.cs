namespace Rules.Framework
{
    /// <summary>
    /// The priority options available to influence the priority at which a new rule is added to
    /// data source.
    /// </summary>
    public enum PriorityOptions
    {
        /// <summary>
        /// Specifies to add rule positioned at top of priority values (smallest priority value).
        /// </summary>
        AtTop = 1,

        /// <summary>
        /// Specifies to add rule positioned at bottom of priority values (largest priority value).
        /// </summary>
        AtBottom = 2,

        /// <summary>
        /// Specifies to add rule positioned at existent rule's name. All subsequent rules
        /// (including the one referenced) are increased on priority by one.
        /// </summary>
        AtRuleName = 3,

        /// <summary>
        /// Specifies to add rule positioned at specified priority number given. Any existent rules
        /// at priority number given or superior are increase on priority by one.
        /// </summary>
        AtPriorityNumber = 4
    }
}