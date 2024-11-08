namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;

    internal sealed class FluentConditionNodeBuilder<TCondition> : IFluentConditionNodeBuilder<TCondition>
    {
        private readonly List<IConditionNode> conditions;
        private readonly LogicalOperators logicalOperator;

        public FluentConditionNodeBuilder(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;
            this.conditions = new List<IConditionNode>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IFluentConditionNodeBuilder<TCondition> And(
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
            this.conditions.Add(composedConditionNode);
            return this;
        }

        public IConditionNode Build()
        {
            return new ComposedConditionNode(this.logicalOperator, this.conditions);
        }

        public IFluentConditionNodeBuilder<TCondition> Condition(IConditionNode conditionNode)
        {
            this.conditions.Add(conditionNode);
            return this;
        }

        public IFluentConditionNodeBuilder<TCondition> Or(
                    Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
            this.conditions.Add(composedConditionNode);
            return this;
        }

        public IFluentConditionNodeBuilder<TCondition> Value<T>(TCondition condition, Operators condOperator, T operand)
        {
            var conditionAsString = GenericConversions.Convert(condition);
            var valueConditionNode = ConditionNodeFactory.CreateValueNode(conditionAsString, condOperator, operand);
            this.conditions.Add(valueConditionNode);
            return this;
        }
    }
}