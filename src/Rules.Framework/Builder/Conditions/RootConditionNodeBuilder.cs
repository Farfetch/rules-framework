namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    internal sealed class RootConditionNodeBuilder<TConditionType> : IRootConditionNodeBuilder<TConditionType>
    {
        public IConditionNode<TConditionType> And(
            Func<IFluentConditionNodeBuilder<TConditionType>, IFluentConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory<TConditionType>.CreateComposedNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode<TConditionType> Or(
            Func<IFluentConditionNodeBuilder<TConditionType>, IFluentConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory<TConditionType>.CreateComposedNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode<TConditionType> Value<TDataType>(
            TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            return ConditionNodeFactory<TConditionType>.CreateValueNode(conditionType, condOperator, operand);
        }
    }
}