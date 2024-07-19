namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework;
    using Rules.Framework.Generic;

    internal sealed class RootConditionNodeBuilder : IRootConditionNodeBuilder
    {
        public IConditionNode And(
            Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode Condition(IConditionNode conditionNode)
        {
            return conditionNode;
        }

        public IConditionNode Or(
                    Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode Value<TDataType>(
            string conditionType, Operators condOperator, TDataType operand)
        {
            var conditionTypeAsString = GenericConversions.Convert(conditionType);
            return ConditionNodeFactory.CreateValueNode(conditionTypeAsString, condOperator, operand);
        }
    }
}