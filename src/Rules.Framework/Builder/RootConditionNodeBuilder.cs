namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    internal sealed class RootConditionNodeBuilder<TConditionType> : IRootConditionNodeBuilder<TConditionType>
    {
        public IConditionNode<TConditionType> And(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode<TConditionType> Or(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode<TConditionType> Value<TDataType>(
            TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            return ConditionNodeFactory.CreateValueNode(conditionType, condOperator, operand);
        }
    }
}