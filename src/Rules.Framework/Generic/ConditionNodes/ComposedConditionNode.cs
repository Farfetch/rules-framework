namespace Rules.Framework.Generic.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;

    /// <summary>
    /// A composed condition node which aggregates a set of child condition nodes and defines a
    /// logical operator to apply to them.
    /// </summary>
    /// <typeparam name="TConditionNode">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    [DebuggerDisplay("Composed condition: apply {LogicalOperator.ToString(),nq} operator for {System.Linq.Enumerable.Count(ChildConditionNodes),nq} nodes")]
    public class ComposedConditionNode<TConditionNode> : IConditionNode<TConditionNode>
    {
        private readonly ComposedConditionNode composedConditionNode;
        private List<IConditionNode<TConditionNode>>? children;

        /// <summary>
        /// Creates a new <see cref="ComposedConditionNode{TConditionNode}"/>.
        /// </summary>
        /// <param name="composedConditionNode">The composed condition node.</param>
        public ComposedConditionNode(ComposedConditionNode composedConditionNode)
        {
            this.composedConditionNode = composedConditionNode;
        }

        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        public IEnumerable<IConditionNode<TConditionNode>> ChildConditionNodes
        {
            get
            {
                this.children ??= this.composedConditionNode.ChildConditionNodes
                    .Select(cn => cn.ToGenericConditionNode<TConditionNode>())
                    .ToList();

                return this.children;
            }
        }

        /// <summary>
        /// Gets the logical operator to apply between child condition nodes.
        /// </summary>
        public LogicalOperators LogicalOperator => this.composedConditionNode.LogicalOperator;

        /// <summary>
        /// Gets the condition node properties.
        /// </summary>
        public IDictionary<string, object> Properties => this.composedConditionNode.Properties;

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public IConditionNode<TConditionNode> Clone()
            => new ComposedConditionNode<TConditionNode>((ComposedConditionNode)this.composedConditionNode.Clone());

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is ComposedConditionNode<TConditionNode> node && EqualityComparer<ComposedConditionNode>.Default.Equals(this.composedConditionNode, node.composedConditionNode);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => this.composedConditionNode.GetHashCode();
    }
}