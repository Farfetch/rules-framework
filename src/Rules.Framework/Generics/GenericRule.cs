namespace Rules.Framework.Generics
{
    using System;

    /// <summary>
    /// Defines a generic rule
    /// </summary>
    public sealed class GenericRule
    {
        /// <summary>
        /// Gets and sets the if the rules ia active.
        /// </summary>
        public bool Active { get; internal set; }

        /// <summary>
        /// Gets the content which contains the rule content.
        /// </summary>
        public object Content { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin { get; internal set; }

        /// <summary>
        /// Gets and sets the date from which the rule ceases to be applicable.
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