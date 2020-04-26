namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    /// <summary>
    /// A condition node with a decimal data type.
    /// </summary>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    [DebuggerDisplay("Decimal condition: <{ConditionType.ToString(),nq}> {Operator} {Operand}")]
    public class DecimalConditionNode<TConditionType> : ValueConditionNodeTemplate<decimal, TConditionType>
    {
        /// <summary>
        /// Creates a new <see cref="DecimalConditionNode{TConditionType}"/>.
        /// </summary>
        /// <param name="conditionType">the condition type.</param>
        /// <param name="operator">the operator.</param>
        /// <param name="operand">the operand.</param>
        public DecimalConditionNode(TConditionType conditionType, Operators @operator, decimal operand)
            : base(conditionType, @operator, operand)
        {
        }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public override DataTypes DataType => DataTypes.Decimal;

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public override IConditionNode<TConditionType> Clone()
            => new DecimalConditionNode<TConditionType>(this.ConditionType, this.Operator, this.Operand);
    }
}