namespace Rules.Framework.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Rules.Framework;
    using Rules.Framework.Core;

    /// <summary>
    /// A generic implementation for a valued condition node.
    /// </summary>
    /// <seealso cref="IValueConditionNode"/>
    [DebuggerDisplay("{DataType.ToString(),nq} condition: <{Condition,nq}> {Operator} {Operand}")]
    public class ValueConditionNode : IValueConditionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="condition">The condition name.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="operand">The operand.</param>
        public ValueConditionNode(DataTypes dataType, string condition, Operators @operator, object operand)
            : this(dataType, condition, @operator, operand, new PropertiesDictionary(Constants.DefaultPropertiesDictionarySize))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="condition">The condition name.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <param name="properties">The properties.</param>
        public ValueConditionNode(DataTypes dataType, string condition, Operators @operator, object operand, IDictionary<string, object> properties)
        {
            this.Condition = condition;
            this.DataType = dataType;
            this.Operand = operand;
            this.Operator = @operator;
            this.Properties = properties;
        }

        /// <inheritdoc/>
        public string Condition { get; }

        /// <inheritdoc/>
        public DataTypes DataType { get; }

        /// <inheritdoc/>
        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        /// <inheritdoc/>
        public object Operand { get; }

        /// <inheritdoc/>
        public Operators Operator { get; }

        /// <inheritdoc/>
        public IDictionary<string, object> Properties { get; }

        /// <inheritdoc/>
        public IConditionNode Clone()
            => new ValueConditionNode(
                this.DataType,
                this.Condition,
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
        public override bool Equals(object obj) => obj is ValueConditionNode node && StringComparer.Ordinal.Equals(this.Condition, node.Condition) && this.DataType == node.DataType && this.LogicalOperator == node.LogicalOperator && EqualityComparer<object>.Default.Equals(this.Operand, node.Operand) && this.Operator == node.Operator && EqualityComparer<IDictionary<string, object>>.Default.Equals(this.Properties, node.Properties);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => HashCode.Combine(this.Condition, this.DataType, this.LogicalOperator, this.Operand, this.Operator, this.Properties);
    }
}