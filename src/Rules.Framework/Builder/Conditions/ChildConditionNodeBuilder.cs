namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class ChildConditionNodeBuilder<TConditionType> : IChildConditionNodeBuilder<TConditionType>
    {
        private readonly List<IConditionNode<TConditionType>> conditions;
        private LogicalOperators logicalOperator;

        public ChildConditionNodeBuilder(LogicalOperators logicOperator)
        {
            this.logicalOperator = logicOperator;
            this.conditions = new List<IConditionNode<TConditionType>>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IChildConditionNodeBuilder<TConditionType> And(
            Func<IChildConditionNodeBuilder<TConditionType>, IChildConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory<TConditionType>.CreateChildNode(LogicalOperators.And, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IConditionNode<TConditionType> Build()
        {
            return new ComposedConditionNode<TConditionType>(this.logicalOperator, this.conditions);
        }

        public IChildConditionNodeBuilder<TConditionType> Or(
            Func<IChildConditionNodeBuilder<TConditionType>, IChildConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory<TConditionType>.CreateChildNode(LogicalOperators.Or, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IChildConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            var valueConditionNode = ConditionNodeFactory<TConditionType>.CreateValueNode(conditionType, condOperator, operand);

            this.conditions.Add(valueConditionNode);

            return this;
        }
    }
}