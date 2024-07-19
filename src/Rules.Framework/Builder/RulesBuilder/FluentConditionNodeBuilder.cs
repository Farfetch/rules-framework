namespace Rules.Framework.Builder.RulesBuilder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;

    internal sealed class FluentConditionNodeBuilder : IFluentConditionNodeBuilder
    {
        private readonly List<IConditionNode> conditions;
        private readonly LogicalOperators logicalOperator;

        public FluentConditionNodeBuilder(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;
            this.conditions = new List<IConditionNode>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IFluentConditionNodeBuilder And(
            Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
            this.conditions.Add(composedConditionNode);
            return this;
        }

        public IConditionNode Build()
        {
            return new ComposedConditionNode(this.logicalOperator, this.conditions);
        }

        public IFluentConditionNodeBuilder Condition(IConditionNode conditionNode)
        {
            this.conditions.Add(conditionNode);
            return this;
        }

        public IFluentConditionNodeBuilder Or(
                    Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
            this.conditions.Add(composedConditionNode);
            return this;
        }

        public IFluentConditionNodeBuilder Value<T>(string condition, Operators condOperator, T operand)
        {
            var conditionTypeAsString = GenericConversions.Convert(condition);
            var valueConditionNode = ConditionNodeFactory.CreateValueNode(conditionTypeAsString, condOperator, operand);
            this.conditions.Add(valueConditionNode);
            return this;
        }
    }
}