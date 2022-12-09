namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class ComposedConditionNodeBuilder<TConditionType> : IComposedConditionNodeBuilder<TConditionType>
    {
        private readonly IConditionNodeBuilder<TConditionType> conditionNodeBuilder;
        private readonly List<IConditionNode<TConditionType>> conditions;
        private LogicalOperators logicalOperator;

        public ComposedConditionNodeBuilder(IConditionNodeBuilder<TConditionType> conditionNodeBuilder)
        {
            this.conditionNodeBuilder = conditionNodeBuilder;
            this.conditions = new List<IConditionNode<TConditionType>>(2); // Most probable number of conditions, so that collection is initialized with right size most times.
        }

        public IComposedConditionNodeBuilder<TConditionType> WithLogicalOperator(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;

            return this;
        }

        public IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> addConditionFunc)
        {
            IConditionNode<TConditionType> conditionNode = addConditionFunc.Invoke(this.conditionNodeBuilder);
            this.conditions.Add(conditionNode);

            return this;
        }

        public IConditionNode<TConditionType> Build()
        {
            return new ComposedConditionNode<TConditionType>(this.logicalOperator, this.conditions);
        }
    }
}
