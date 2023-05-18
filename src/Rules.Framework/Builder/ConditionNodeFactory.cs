namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// Factory for creating condition nodes.
    /// </summary>
    public class ConditionNodeFactory
    {
        /// <summary>
        /// Creates a composed condition node.
        /// </summary>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="conditionFunc">
        /// The function containing the logic for the composed condition node.
        /// </param>
        /// <returns></returns>
        public static IConditionNode<TConditionType> CreateComposedNode<TConditionType>(
            LogicalOperators logicalOperator,
            Func<IFluentComposedConditionNodeBuilder<TConditionType>, IFluentComposedConditionNodeBuilder<TConditionType>> conditionFunc)
        {
            var composedConditionNodeBuilder = new FluentComposedConditionNodeBuilder<TConditionType>(logicalOperator);

            var composedConditionNode = conditionFunc
                .Invoke(composedConditionNodeBuilder)
                .Build();

            return composedConditionNode;
        }

        /// <summary>
        /// Creates a value condition node.
        /// </summary>
        /// <param name="conditionType">The condition type.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        public static IConditionNode<TConditionType> CreateValueNode<TConditionType, TDataType>(
            TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            return new ValueConditionNodeBuilder<TConditionType, TDataType>(conditionType, condOperator, operand)
                .Build();
        }
    }
}