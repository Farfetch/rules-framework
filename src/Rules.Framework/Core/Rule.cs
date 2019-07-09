namespace Rules.Framework.Core
{
    using System;

    /// <summary>
    /// Defines a rule.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    public class Rule<TContentType, TConditionType>
    {
        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public ContentContainer<TContentType> ContentContainer { get; internal set; }

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
        public IConditionNode<TConditionType> RootCondition { get; internal set; }
    }
}