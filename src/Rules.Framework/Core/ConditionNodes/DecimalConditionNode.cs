namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    [DebuggerDisplay("Decimal condition: <{ConditionType.ToString(),nq}> {Operator} {Operand}")]
    public class DecimalConditionNode<TConditionType> : ValueConditionNodeTemplate<decimal, TConditionType>
    {
        public DecimalConditionNode(TConditionType conditionType, Operators @operator, decimal operand)
            : base(conditionType, @operator, operand)
        {
        }

        public override DataTypes DataType => DataTypes.Decimal;
    }
}