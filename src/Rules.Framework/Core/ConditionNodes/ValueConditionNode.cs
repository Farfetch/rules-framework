namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    /// <summary>
    /// A generic implementation for a valued condition node.
    /// </summary>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    /// <seealso cref="Rules.Framework.Core.ConditionNodes.IValueConditionNode{TConditionType}"/>
    [DebuggerDisplay("{DataType.ToString(),nq} condition: <{ConditionType.ToString(),nq}> {Operator} {Operand}")]
    public class ValueConditionNode<TConditionType> : IValueConditionNode<TConditionType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode{TConditionType}"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="conditionType">Type of the condition.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <param name="internalId">The internal identificator</param>
        public ValueConditionNode(DataTypes dataType, TConditionType conditionType, Operators @operator, object operand, object internalId = null)
        {
            this.ConditionType = conditionType;
            this.DataType = dataType;
            this.Operand = operand;
            this.Operator = @operator;
            this.InternalId = internalId;
        }

        /// <summary>
        /// Gets the condition node type.
        /// </summary>
        public TConditionType ConditionType { get; }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public DataTypes DataType { get; }

        /// <summary>
        /// the internal identifier associated with this value condition node
        /// </summary>
        public object InternalId { get; }

        /// <summary>
        /// Gets the logical operator to apply to condition node.
        /// </summary>
        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        /// <value>The operand.</value>
        public object Operand { get; }

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        public Operators Operator { get; }

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public IConditionNode<TConditionType> Clone()
            => new ValueConditionNode<TConditionType>(this.DataType, this.ConditionType, this.Operator, this.Operand);
    }
}