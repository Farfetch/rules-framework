namespace Rules.Framework.Generic
{
    using Rules.Framework.Core;

    public class GenericConditionNode<ConditionType>
    {
        /// <summary>
        /// Gets the logical operator to apply to condition node.
        /// </summary>
        public LogicalOperators LogicalOperator { get; internal set; }
    }
}