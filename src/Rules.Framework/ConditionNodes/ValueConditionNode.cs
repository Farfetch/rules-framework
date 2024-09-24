namespace Rules.Framework.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Rules.Framework;
    using Rules.Framework.Core;
    using Rules.Framework.Generic.ConditionNodes;

    /// <summary>
    /// A generic implementation for a valued condition node.
    /// </summary>
    /// <seealso cref="Generic.ConditionNodes.IValueConditionNode{TConditionType}"/>
    [DebuggerDisplay("{DataType.ToString(),nq} condition: <{ConditionType,nq}> {Operator} {Operand}")]
    public class ValueConditionNode : IValueConditionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode{TConditionType}"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="conditionType">Type of the condition.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="operand">The operand.</param>
        public ValueConditionNode(DataTypes dataType, string conditionType, Operators @operator, object operand)
            : this(dataType, conditionType, @operator, operand, new PropertiesDictionary(Constants.DefaultPropertiesDictionarySize))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode{TConditionType}"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="conditionType">Type of the condition.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <param name="properties">The properties.</param>
        public ValueConditionNode(DataTypes dataType, string conditionType, Operators @operator, object operand, IDictionary<string, object> properties)
        {
            this.ConditionType = conditionType;
            this.DataType = dataType;
            this.Operand = operand;
            this.Operator = @operator;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the condition node type.
        /// </summary>
        public string ConditionType { get; }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public DataTypes DataType { get; }

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
        /// Gets the condition node properties.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public IConditionNode Clone()
            => new ValueConditionNode(
                this.DataType,
                this.ConditionType,
                this.Operator,
                this.Operand,
                new PropertiesDictionary(this.Properties));

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is ValueConditionNode node && StringComparer.Ordinal.Equals(this.ConditionType, node.ConditionType) && this.DataType == node.DataType && this.LogicalOperator == node.LogicalOperator && EqualityComparer<object>.Default.Equals(this.Operand, node.Operand) && this.Operator == node.Operator && EqualityComparer<IDictionary<string, object>>.Default.Equals(this.Properties, node.Properties);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => HashCode.Combine(this.ConditionType, this.DataType, this.LogicalOperator, this.Operand, this.Operator, this.Properties);
    }
}