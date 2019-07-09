namespace Rules.Framework
{
    /// <summary>
    /// Defines a condition to filter rules.
    /// </summary>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    public class Condition<TConditionType>
    {
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