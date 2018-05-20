namespace Rules.Framework.Core.ConditionNodes
{
    public class DecimalConditionNode<TConditionType> : ValueConditionNodeTemplate<decimal, TConditionType>
    {
        public override DataTypes DataType => DataTypes.Decimal;
    }
}