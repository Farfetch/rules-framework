namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class ChildComposedConditionNodeBuilder<TConditionType> : IChildComposedConditionNodeBuilder<TConditionType>
    {
        private readonly List<IConditionNode<TConditionType>> conditions;
        private readonly LogicalOperators logicalOperator;

        public ChildComposedConditionNodeBuilder(LogicalOperators logicOperator)
        {
            this.logicalOperator = logicOperator;
            this.conditions = new List<IConditionNode<TConditionType>>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IChildComposedConditionNodeBuilder<TConditionType> And(
            Func<IChildComposedConditionNodeBuilder<TConditionType>, IChildComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory<TConditionType>.CreateChildNode(LogicalOperators.And, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IConditionNode<TConditionType> Build()
        {
            return new ComposedConditionNode<TConditionType>(this.logicalOperator, this.conditions);
        }

        public IChildComposedConditionNodeBuilder<TConditionType> Or(
            Func<IChildComposedConditionNodeBuilder<TConditionType>, IChildComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory<TConditionType>.CreateChildNode(LogicalOperators.Or, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IChildComposedConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            var valueConditionNode = ConditionNodeFactory<TConditionType>.CreateValueNode(conditionType, condOperator, operand);

            this.conditions.Add(valueConditionNode);

            return this;
        }
    }
}