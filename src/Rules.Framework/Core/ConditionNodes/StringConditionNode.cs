namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    /// <summary>
    /// A condition node with a string data type.
    /// </summary>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    [DebuggerDisplay("String condition: <{ConditionType.ToString(),nq}> {Operator} '{Operand}'")]
    public class StringConditionNode<TConditionType> : ValueConditionNodeTemplate<string, TConditionType>
    {
        /// <summary>
        /// Creates a new <see cref="StringConditionNode{TConditionType}"/>.
        /// </summary>
        /// <param name="conditionType">the condition type.</param>
        /// <param name="operator">the operator.</param>
        /// <param name="operand">the operand.</param>
        public StringConditionNode(TConditionType conditionType, Operators @operator, string operand)
            : base(conditionType, @operator, operand)
        {
        }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        public override DataTypes DataType => DataTypes.String;

        /// <summary>
        /// Clones the condition node into a different instance.
        /// </summary>
        /// <returns></returns>
        public override IConditionNode<TConditionType> Clone()
            => new StringConditionNode<TConditionType>(this.ConditionType, this.Operator, this.Operand);
    }
}