namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    /// <summary>
    /// A condition node with a integer data type.
    /// </summary>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    [DebuggerDisplay("Integer condition: <{ConditionType.ToString(),nq}> {Operator} {Operand}")]
    public class IntegerConditionNode<TConditionType> : ValueConditionNodeTemplate<int, TConditionType>
    {
        /// <summary>
        /// Creates a new <see cref="IntegerConditionNode{TConditionType}"/>.
        /// </summary>
        /// <param name="conditionType">the condition type.</param>
        /// <param name="operator">the operator.</param>
        /// <param name="operand">the operand.</param>
        public IntegerConditionNode(TConditionType conditionType, Operators @operator, int operand)
            : base(conditionType, @operator, operand)
        {
        }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public override DataTypes DataType => DataTypes.Integer;

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public override IConditionNode<TConditionType> Clone()
            => new IntegerConditionNode<TConditionType>(this.ConditionType, this.Operator, this.Operand);
    }
}