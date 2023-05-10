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

        [Obsolete("This way of adding conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        public IComposedConditionNodeBuilder<TConditionType> AddCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc)
        {
            var conditionNode = conditionFunc.Invoke(this.conditionNodeBuilder);

            this.conditions.Add(conditionNode);

            return this;
        }

        public IComposedConditionNodeBuilder<TConditionType> And(
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return this.AddComposedCondition(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode<TConditionType> Build()
        {
            return new ComposedConditionNode<TConditionType>(this.logicalOperator, this.conditions);
        }

        public IComposedConditionNodeBuilder<TConditionType> Or(
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            return this.AddComposedCondition(LogicalOperators.Or, conditionFunc);
        }

        public IComposedConditionNodeBuilder<TConditionType> Value<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            return this.AddValueCondition(conditionType, condOperator, operand);
        }

        [Obsolete("This way of composing conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        public IComposedConditionNodeBuilder<TConditionType> WithLogicalOperator(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;

            return this;
        }

        private IComposedConditionNodeBuilder<TConditionType> AddComposedCondition(
            LogicalOperators logicOperator,
            Func<IComposedConditionNodeBuilder<TConditionType>, IComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNodeBuilder = new ConditionNodeBuilder<TConditionType>()
                .AsComposed()
                .WithLogicalOperator(logicOperator);

            var composedConditionNode = conditionFunc
                .Invoke(composedConditionNodeBuilder)
                .Build();

            this.conditions.Add(composedConditionNode);

            return this;
        }

        private IComposedConditionNodeBuilder<TConditionType> AddValueCondition<TDataType>(TConditionType conditionType, Operators condOperator, TDataType operand)
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
    }
}