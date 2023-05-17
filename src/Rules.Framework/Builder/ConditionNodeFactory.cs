namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    internal static class ConditionNodeFactory<TConditionType>
    {
        public static IConditionNode<TConditionType> CreateComposedNode(
            LogicalOperators logicalOperator,
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNodeBuilder = new FluentComposedConditionNodeBuilder<TConditionType>(logicalOperator);

            var composedConditionNode = conditionFunc
                .Invoke(composedConditionNodeBuilder)
                .Build();

            return composedConditionNode;
        }

        public static IConditionNode<TConditionType> CreateValueNode<TDataType>(
            TConditionType conditionType,
            Operators condOperator,
            TDataType operand)
        {
            return new ValueConditionNodeBuilder<TConditionType>(conditionType)
                .OfDataType<TDataType>()
                .WithComparisonOperator(condOperator)
                .SetOperand(operand)
                .Build();
        }
    }
}