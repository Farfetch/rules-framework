using System.Diagnostics;

namespace Rules.Framework.Core.ConditionNodes
{
    [DebuggerDisplay("String condition: <?> {Operator} '{Operand}'")]
    public class StringConditionNode<TConditionType> : ValueConditionNodeTemplate<string, TConditionType>
    {
        public override DataTypes DataType => DataTypes.String;
    }
}