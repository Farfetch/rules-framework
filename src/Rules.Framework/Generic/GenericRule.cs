namespace Rules.Framework.Generic
{
    using System;

    /// <summary>
    /// Defines a generic rule
    /// </summary>
    public class GenericRule
    {
        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public object ContentContainer { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule ceases to be applicable.
        /// </summary>
        public DateTime? DateEnd { get; internal set; }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the rule priority compared to other rules (preferrably it is unique).
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Gets the rule root condition. This property is null when rule has no conditions.
        /// </summary>
        public GenericConditionNode RootCondition { get; internal set; }
    }
}