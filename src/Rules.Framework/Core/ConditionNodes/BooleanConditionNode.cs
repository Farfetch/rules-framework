namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    /// <summary>
    /// A condition node with a boolean data type.
    /// </summary>
    /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
    [DebuggerDisplay("Boolean condition: <{ConditionType.ToString(),nq}> {Operator} '{Operand}'")]
    public class BooleanConditionNode<TConditionType> : ValueConditionNodeTemplate<bool, TConditionType>
    {
        /// <summary>
        /// Creates a new <see cref="BooleanConditionNode{TConditionType}"/>.
        /// </summary>
        /// <param name="conditionType">the condition type.</param>
        /// <param name="operator">the operator.</param>
        /// <param name="operand">the operand.</param>
        public BooleanConditionNode(TConditionType conditionType, Operators @operator, bool operand)
            : base(conditionType, @operator, operand)
        {
        }

        /// <summary>
        /// The condition node data type.
        /// </summary>
        public override DataTypes DataType => DataTypes.Boolean;
    }
}