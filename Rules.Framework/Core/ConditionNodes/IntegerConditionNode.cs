using System.Diagnostics;

namespace Rules.Framework.Core.ConditionNodes
{
    [DebuggerDisplay("Integer condition: <?> {Operator} {Operand}")]
    public class IntegerConditionNode<TConditionType> : ValueConditionNodeTemplate<int, TConditionType>
    {
        public override DataTypes DataType => DataTypes.Integer;
    }
}