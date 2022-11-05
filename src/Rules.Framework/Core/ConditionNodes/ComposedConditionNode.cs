namespace Rules.Framework.Core.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// A composed condition node which aggregates a set of child condition nodes and defines a logical operator to apply to them.
    /// </summary>
    /// <typeparam name="TConditionNode">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    [DebuggerDisplay("Composed condition: apply {LogicalOperator.ToString(),nq} operator for {System.Linq.Enumerable.Count(ChildConditionNodes),nq} nodes")]
    public class ComposedConditionNode<TConditionNode> : IConditionNode<TConditionNode>
    {
        /// <summary>
        /// Creates a new <see cref="ComposedConditionNode{TConditionNode}"/>.
        /// </summary>
        /// <param name="logicalOperator">the logical operator.</param>
        /// <param name="childConditionNodes">the set of child condition nodes.</param>
        public ComposedConditionNode(LogicalOperators logicalOperator, IEnumerable<IConditionNode<TConditionNode>> childConditionNodes)
        {
            this.LogicalOperator = logicalOperator;
            this.ChildConditionNodes = childConditionNodes;
            this.Properties = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        public IEnumerable<IConditionNode<TConditionNode>> ChildConditionNodes { get; }

        /// <summary>
        /// Gets the logical operator to apply between child condition nodes.
        /// </summary>
        public LogicalOperators LogicalOperator { get; }

        /// <summary>
        /// Gets the condition node properties.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public IConditionNode<TConditionNode> Clone()
            => new ComposedConditionNode<TConditionNode>(this.LogicalOperator, this.ChildConditionNodes.Select(cn => cn.Clone()).ToList().AsReadOnly());
    }
}