namespace Rules.Framework.Builder.Generic
{
    using System;
    using Rules.Framework;
    using Rules.Framework.Generic;

    internal sealed class RootConditionNodeBuilder<TConditionType> : IRootConditionNodeBuilder<TConditionType>
    {
        public IConditionNode And(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode Condition(IConditionNode conditionNode)
        {
            return conditionNode;
        }

        public IConditionNode Or(
                    Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode Value<TDataType>(
            TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            var conditionTypeAsString = GenericConversions.Convert(conditionType);
            return ConditionNodeFactory.CreateValueNode(conditionTypeAsString, condOperator, operand);
        }
    }
}