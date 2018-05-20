namespace Rules.Framework.Core.ConditionNodes
{
    public class BooleanConditionNode<TConditionType> : ValueConditionNodeTemplate<bool, TConditionType>
    {
        public override DataTypes DataType => DataTypes.Boolean;
    }
}