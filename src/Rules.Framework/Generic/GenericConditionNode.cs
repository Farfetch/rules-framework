namespace Rules.Framework.Generic
{
    using Rules.Framework.Core;

    /// <summary>
    /// Generic condition node
    /// </summary>
    /// <typeparam name="ConditionType">The type of the ondition type.</typeparam>
    public class GenericConditionNode<ConditionType>
    {
        /// <summary>
        /// Gets the logical operator to apply to condition node.
        /// </summary>
        public LogicalOperators LogicalOperator { get; internal set; }
    }
}