namespace Rules.Framework
{
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
        /// <param name="Type">The type of the condition.</param>
        /// <param name="Value">The value of the condition.</param>
        public Condition(TConditionType Type, object Value)
        {
            this.Type = Type;
            this.Value = Value;
        }

        /// <summary>
        /// Creates a Condition.
        /// </summary>
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