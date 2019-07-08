namespace Rules.Framework.Core
{
    public interface IConditionNode<TConditionType>
    {
        LogicalOperators LogicalOperator { get; }
    }
}