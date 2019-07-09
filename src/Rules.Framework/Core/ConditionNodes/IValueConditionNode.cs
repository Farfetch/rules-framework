namespace Rules.Framework.Core.ConditionNodes
{
    public interface IValueConditionNode<TConditionType> : IConditionNode<TConditionType>
    {
        TConditionType ConditionType { get; }

        DataTypes DataType { get; }

        Operators Operator { get; }
    }
}