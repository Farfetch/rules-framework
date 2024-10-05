namespace Rules.Framework.Builder.RulesBuilder
{
    using System;
    using Rules.Framework;
    using Rules.Framework.Generic;

    internal sealed class RootConditionNodeBuilder : IRootConditionNodeBuilder
    {
        public IConditionNode And(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode Condition(IConditionNode conditionNode)
        {
            return conditionNode;
        }

        public IConditionNode Or(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode Value<T>(string condition, Operators condOperator, T operand)
        {
            var conditionAsString = GenericConversions.Convert(condition);
            return ConditionNodeFactory.CreateValueNode(conditionAsString, condOperator, operand);
        }
    }
}