namespace Rules.Framework.Generics
{
    using Rules.Framework.Core;

    /// <summary>
    /// Defines generic condition node
    /// </summary>
    public class GenericConditionNode
    {
        /// <summary>
        /// Gets the logical operator to apply to condition node.
        /// </summary>
        public LogicalOperators LogicalOperator { get; internal set; }
    }
}