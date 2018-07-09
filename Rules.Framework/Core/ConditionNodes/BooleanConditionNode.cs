namespace Rules.Framework.Core.ConditionNodes
{
    public class BooleanConditionNode<TConditionType> : ValueConditionNodeTemplate<bool, TConditionType>
    {
        public BooleanConditionNode(TConditionType conditionType, Operators @operator, bool operand)
            : base(conditionType, @operator, operand)
        {
        }

        public override DataTypes DataType => DataTypes.Boolean;
    }
}