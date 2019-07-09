namespace Rules.Framework.Core.ConditionNodes
{
    using System;

    /// <summary>
    /// Defines the template implementation for a condition node based on a value comparison.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    public abstract class ValueConditionNodeTemplate<T, TConditionType> : IValueConditionNode<TConditionType>
        where T : IComparable<T>
    {
        /// <summary>
        /// Creates a new <see cref="ValueConditionNodeTemplate{T, TConditionType}"/>.
        /// </summary>
        /// <param name="conditionType"></param>
        /// <param name="operator"></param>
        /// <param name="operand"></param>
        protected ValueConditionNodeTemplate(TConditionType conditionType, Operators @operator, T operand)
        {
            this.ConditionType = conditionType;
            this.Operand = operand;
            this.Operator = @operator;
        }

        /// <summary>
        /// Gets the condition type.
        /// </summary>
        public TConditionType ConditionType { get; }

        /// <summary>
        /// Gets the condition's data type.
        /// </summary>
        public abstract DataTypes DataType { get; }

        /// <summary>
        /// Gets the condition's logical operator. (Always Eval for value condition nodes.)
        /// </summary>
        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        public T Operand { get; }

        /// <summary>
        /// Gets the condition's operator to perform value comparison.
        /// </summary>
        public Operators Operator { get; }
    }
}