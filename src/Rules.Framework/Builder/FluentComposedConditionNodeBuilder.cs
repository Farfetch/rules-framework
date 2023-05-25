namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class FluentComposedConditionNodeBuilder<TConditionType> : IFluentComposedConditionNodeBuilder<TConditionType>
    {
        private readonly List<IConditionNode<TConditionType>> conditions;
        private readonly LogicalOperators logicalOperator;

        public FluentComposedConditionNodeBuilder(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;
            this.conditions = new List<IConditionNode<TConditionType>>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IFluentComposedConditionNodeBuilder<TConditionType> And(
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IConditionNode<TConditionType> Build()
        {
            return new ComposedConditionNode<TConditionType>(this.logicalOperator, this.conditions);
        }

        public IFluentComposedConditionNodeBuilder<TConditionType> Condition(IConditionNode<TConditionType> valueConditionNode)
        {
            this.conditions.Add(valueConditionNode);

            return this;
        }

        public IFluentComposedConditionNodeBuilder<TConditionType> Or(
                    Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IFluentComposedConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            var valueConditionNode = ConditionNodeFactory.CreateValueNode(conditionType, condOperator, operand);

            this.conditions.Add(valueConditionNode);

            return this;
        }
    }
}