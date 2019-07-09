namespace Rules.Framework.Core.ConditionNodes
{
    using System.Diagnostics;

    [DebuggerDisplay("String condition: <{ConditionType.ToString(),nq}> {Operator} '{Operand}'")]
    public class StringConditionNode<TConditionType> : ValueConditionNodeTemplate<string, TConditionType>
    {
        public StringConditionNode(TConditionType conditionType, Operators @operator, string operand)
            : base(conditionType, @operator, operand)
        {
        }

        public override DataTypes DataType => DataTypes.String;
    }
}