namespace Rules.Framework.Generic.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;

    /// <summary>
    /// A generic implementation for a valued condition node.
    /// </summary>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    /// <seealso cref="IValueConditionNode{TConditionType}"/>
    [DebuggerDisplay("{DataType.ToString(),nq} condition: <{ConditionType.ToString(),nq}> {Operator} {Operand}")]
    public class ValueConditionNode<TConditionType> : IValueConditionNode<TConditionType>
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

        /// <summary>
        /// Gets the condition node type.
        /// </summary>
        public TConditionType ConditionType => GenericConversions.Convert<TConditionType>(valueConditionNode.ConditionType);

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public DataTypes DataType => this.valueConditionNode.DataType;

        /// <summary>
        /// Gets the logical operator to apply to condition node.
        /// </summary>
        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        /// <value>The operand.</value>
        public object Operand => this.valueConditionNode.Operand;

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        public Operators Operator => this.valueConditionNode.Operator;

        /// <summary>
        /// Gets the condition node properties.
        /// </summary>
        public IDictionary<string, object> Properties => this.valueConditionNode.Properties;

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public IConditionNode<TConditionType> Clone()
            => new ValueConditionNode<TConditionType>((ValueConditionNode)this.valueConditionNode.Clone());

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is ValueConditionNode<TConditionType> node && EqualityComparer<ValueConditionNode>.Default.Equals(this.valueConditionNode, node.valueConditionNode);

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