namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    internal static class ConditionNodeFactory<TConditionType>
    {
        public static IConditionNode<TConditionType> CreateComposedNode(
            LogicalOperators logicOperator,
            Func<IFluentConditionNodeBuilder<TConditionType>, IFluentConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var childConditionNodeBuilder = new FluentConditionNodeBuilder<TConditionType>(logicOperator);

            var childConditionNode = conditionFunc
                .Invoke(childConditionNodeBuilder)
                .Build();

            return childConditionNode;
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