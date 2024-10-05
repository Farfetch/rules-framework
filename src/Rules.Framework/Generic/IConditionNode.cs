namespace Rules.Framework.Generic
{
    using System.Collections.Generic;
    using Rules.Framework;

    /// <summary>
    /// Defines the interface contract for a rule's condition node.
    /// </summary>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IConditionNode<TCondition>
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
        IConditionNode<TCondition> Clone();
    }
}