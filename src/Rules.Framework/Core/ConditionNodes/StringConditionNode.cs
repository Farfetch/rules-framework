using System.Diagnostics;

namespace Rules.Framework.Core.ConditionNodes
{
    [DebuggerDisplay("String condition: <?> {Operator} '{Operand}'")]
    public class StringConditionNode<TConditionType> : ValueConditionNodeTemplate<string, TConditionType>
    {
        public StringConditionNode(TConditionType conditionType, Operators @operator, string operand)
            : base(conditionType, @operator, operand)
        {
        }

        public override DataTypes DataType => DataTypes.String;
    }
}