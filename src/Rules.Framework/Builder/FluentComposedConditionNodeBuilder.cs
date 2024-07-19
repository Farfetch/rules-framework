namespace Rules.Framework.Builder.Generic
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic;

    internal sealed class FluentComposedConditionNodeBuilder : IFluentComposedConditionNodeBuilder
    {
        private readonly List<IConditionNode> conditions;
        private readonly LogicalOperators logicalOperator;

        public FluentComposedConditionNodeBuilder(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;
            this.conditions = new List<IConditionNode>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IFluentComposedConditionNodeBuilder And(
            Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IConditionNode Build()
        {
            return new ComposedConditionNode(this.logicalOperator, this.conditions);
        }

        public IFluentComposedConditionNodeBuilder Condition(IConditionNode conditionNode)
        {
            this.conditions.Add(conditionNode);

            return this;
        }

        public IFluentComposedConditionNodeBuilder Or(
                    Func<IFluentComposedConditionNodeBuilder, IFluentComposedConditionNodeBuilder> conditionFunc)
        {
            var composedConditionNode = ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);

            this.conditions.Add(composedConditionNode);

            return this;
        }

        public IFluentComposedConditionNodeBuilder Value<TDataType>(string conditionType, Operators condOperator, TDataType operand)
        {
            var conditionTypeAsString = GenericConversions.Convert(conditionType);
            var valueConditionNode = ConditionNodeFactory.CreateValueNode(conditionTypeAsString, condOperator, operand);

            this.conditions.Add(valueConditionNode);

            return this;
        }
    }
}