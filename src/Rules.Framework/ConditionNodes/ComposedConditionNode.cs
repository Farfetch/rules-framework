namespace Rules.Framework.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Rules.Framework;
    using Rules.Framework.Core;

    /// <summary>
    /// A composed condition node which aggregates a set of child condition nodes and defines a
    /// logical operator to apply to them.
    /// </summary>
    [DebuggerDisplay("Composed condition: apply {LogicalOperator.ToString(),nq} operator for {System.Linq.Enumerable.Count(ChildConditionNodes),nq} nodes")]
    public class ComposedConditionNode : IConditionNode
    {
        /// <summary>
        /// Creates a new <see cref="ComposedConditionNode"/>.
        /// </summary>
        /// <param name="logicalOperator">the logical operator.</param>
        /// <param name="childConditionNodes">the set of child condition nodes.</param>
        public ComposedConditionNode(
            LogicalOperators logicalOperator,
            IEnumerable<IConditionNode> childConditionNodes)
            : this(logicalOperator, childConditionNodes, new PropertiesDictionary(Constants.DefaultPropertiesDictionarySize))
        {
        }

        /// <summary>
        /// Creates a new <see cref="ComposedConditionNode"/>.
        /// </summary>
        /// <param name="logicalOperator">the logical operator.</param>
        /// <param name="childConditionNodes">the set of child condition nodes.</param>
        /// <param name="properties">the properties.</param>
        public ComposedConditionNode(
            LogicalOperators logicalOperator,
            IEnumerable<IConditionNode> childConditionNodes,
            IDictionary<string, object> properties)
        {
            this.LogicalOperator = logicalOperator;
            this.ChildConditionNodes = childConditionNodes;
            this.Properties = properties;
        }

        /// <inheritdoc/>
        public IEnumerable<IConditionNode> ChildConditionNodes { get; }

        /// <inheritdoc/>
        public LogicalOperators LogicalOperator { get; }

        /// <inheritdoc/>
        public IDictionary<string, object> Properties { get; }

        /// <inheritdoc/>
        public IConditionNode Clone()
            => new ComposedConditionNode(
                this.LogicalOperator,
                this.ChildConditionNodes.Select(cn => cn.Clone()).ToList().AsReadOnly(),
                new PropertiesDictionary(this.Properties));

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is ComposedConditionNode node && EqualityComparer<IEnumerable<IConditionNode>>.Default.Equals(this.ChildConditionNodes, node.ChildConditionNodes) && this.LogicalOperator == node.LogicalOperator && EqualityComparer<IDictionary<string, object>>.Default.Equals(this.Properties, node.Properties);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => HashCode.Combine(this.ChildConditionNodes, this.LogicalOperator, this.Properties);
    }
}