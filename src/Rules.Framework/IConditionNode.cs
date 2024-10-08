namespace Rules.Framework
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interface contract for a rule's condition node.
    /// </summary>
    public interface IConditionNode
    {
        /// <summary>
        /// Gets the logical operator to apply to condition node.
        /// </summary>
        LogicalOperators LogicalOperator { get; }

        /// <summary>
        /// Gets the condition node properties.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        IConditionNode Clone();
    }
}