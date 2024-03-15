namespace Rules.Framework
{
    using System;

    /// <summary>
    /// Defines a condition to filter rules.
    /// </summary>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public class Condition<TConditionType>
    {
        /// <summary>
        /// Creates a Condition.
        /// </summary>
        /// <param name="type">The type of the condition.</param>
        /// <param name="value">The value of the condition.</param>
        public Condition(TConditionType type, object value)
        {
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Creates a Condition.
        /// </summary>
        [Obsolete("Please use the constructor with parameters instead.")]
        public Condition()
        {
        }

        /// <summary>
        /// Gets or sets the condition type.
        /// </summary>
        public TConditionType Type { get; set; }

        /// <summary>
        /// Gets or sets the condition value.
        /// </summary>
        public object Value { get; set; }
    }
}