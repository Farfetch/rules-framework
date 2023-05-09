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

        public IComposedConditionNodeBuilder<TConditionType> AddComposedCondition(
            LogicalOperators logicOperator,
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> addConditionFunc)
        {
            var composedConditionNodeBuilder = new ConditionNodeBuilder<TConditionType>()
                .AsComposed()
                .WithLogicalOperator(logicOperator);

            var composedConditionBuilder = addConditionFunc.Invoke(composedConditionNodeBuilder);

            this.conditions.Add(composedConditionBuilder.Build());

            return this;
        }

        public IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> addConditionFunc)
        {
            var conditionNode = addConditionFunc.Invoke(this.conditionNodeBuilder);

            this.conditions.Add(conditionNode);

            return this;
        }

        public IComposedConditionNodeBuilder<TConditionType> AddValueCondition<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            var valueCondition = new ConditionNodeBuilder<TConditionType>()
                .AsValued(conditionType)
                .OfDataType<TDataType>()
                .WithComparisonOperator(condOperator)
                .SetOperand(operand)
                .Build();

            this.conditions.Add(valueCondition);

            return this;
        }

        public IConditionNode<TConditionType> Build()
        {
            return new ComposedConditionNode<TConditionType>(this.logicalOperator, this.conditions);
        }

        public IComposedConditionNodeBuilder<TConditionType> WithLogicalOperator(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;

            return this;
        }
    }
}