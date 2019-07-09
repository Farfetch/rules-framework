namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    [DebuggerDisplay("Boolean condition: <{ConditionType.ToString(),nq}> {Operator} '{Operand}'")]
    public class BooleanConditionNode<TConditionType> : ValueConditionNodeTemplate<bool, TConditionType>
    {
        public BooleanConditionNode(TConditionType conditionType, Operators @operator, bool operand)
            : base(conditionType, @operator, operand)
        {
        }

        public override DataTypes DataType => DataTypes.Boolean;
    }
}