namespace Rules.Framework.Generic.ConditionNodes
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;

    /// <summary>
    /// A generic implementation for a valued condition node.
    /// </summary>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    /// <seealso cref="IValueConditionNode{TCondition}"/>
    [DebuggerDisplay("{DataType.ToString(),nq} condition: <{Condition.ToString(),nq}> {Operator} {Operand}")]
    public class ValueConditionNode<TCondition> : IValueConditionNode<TCondition>
    {
        private readonly ValueConditionNode valueConditionNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode{TConditionType}"/> class.
        /// </summary>
        /// <param name="valueConditionNode">The value condition node.</param>
        public ValueConditionNode(ValueConditionNode valueConditionNode)
        {
            this.valueConditionNode = valueConditionNode;
        }

        /// <inheritdoc/>
        public TCondition Condition => GenericConversions.Convert<TCondition>(valueConditionNode.Condition);

        /// <inheritdoc/>
        public DataTypes DataType => this.valueConditionNode.DataType;

        /// <inheritdoc/>
        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        /// <inheritdoc/>
        public object Operand => this.valueConditionNode.Operand;

        /// <inheritdoc/>
        public Operators Operator => this.valueConditionNode.Operator;

        /// <inheritdoc/>
        public IDictionary<string, object> Properties => this.valueConditionNode.Properties;

        /// <inheritdoc/>
        public IConditionNode<TCondition> Clone()
            => new ValueConditionNode<TCondition>((ValueConditionNode)this.valueConditionNode.Clone());

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is ValueConditionNode<TCondition> node && EqualityComparer<ValueConditionNode>.Default.Equals(this.valueConditionNode, node.valueConditionNode);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => this.valueConditionNode.GetHashCode();
    }
}