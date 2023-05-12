namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    internal sealed class RootConditionNodeBuilder<TConditionType> : IRootConditionNodeBuilder<TConditionType>
    {
        public IConditionNode<TConditionType> And(
            Func<IChildConditionNodeBuilder<TConditionType>, IChildConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory<TConditionType>.CreateChildNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode<TConditionType> Or(
            Func<IChildConditionNodeBuilder<TConditionType>, IChildConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory<TConditionType>.CreateChildNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode<TConditionType> Value<TDataType>(
            TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            return ConditionNodeFactory<TConditionType>.CreateValueNode(conditionType, condOperator, operand);
        }
    }
}