namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    /// <summary>
    /// Factory for creating condition nodes.
    /// </summary>
    public static class ConditionNodeFactory
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
            switch (operand)
            {
                case decimal _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Decimal, conditionType, condOperator, operand);

                case IEnumerable<decimal> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayDecimal, conditionType, condOperator, operand);

                case int _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Integer, conditionType, condOperator, operand);

                case IEnumerable<int> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayInteger, conditionType, condOperator, operand);

                case bool _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Boolean, conditionType, condOperator, operand);

                case IEnumerable<bool> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayBoolean, conditionType, condOperator, operand);

                case string _:
                    return new ValueConditionNode<TConditionType>(DataTypes.String, conditionType, condOperator, operand);

                case IEnumerable<string> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayString, conditionType, condOperator, operand);

                default:
                    throw new NotSupportedException($"The data type is not supported: {typeof(TDataType).FullName}.");
            }
        }
    }
}