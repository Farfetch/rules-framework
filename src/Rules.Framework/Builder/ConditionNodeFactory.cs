namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Builder.Generic.RulesBuilder;
    using Rules.Framework.Builder.RulesBuilder;
    using Rules.Framework.ConditionNodes;

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
        public static IConditionNode CreateComposedNode(
            LogicalOperators logicalOperator,
            Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)
        {
            var composedConditionNodeBuilder = new FluentConditionNodeBuilder(logicalOperator);

            var composedConditionNode = conditionFunc
                .Invoke(composedConditionNodeBuilder)
                .Build();

            return composedConditionNode;
        }

        /// <summary>
        /// Creates a composed condition node.
        /// </summary>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="conditionFunc">
        /// The function containing the logic for the composed condition node.
        /// </param>
        /// <returns></returns>
        public static IConditionNode CreateComposedNode<TCondition>(
            LogicalOperators logicalOperator,
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc)
        {
            var composedConditionNodeBuilder = new FluentConditionNodeBuilder<TCondition>(logicalOperator);

            var composedConditionNode = conditionFunc
                .Invoke(composedConditionNodeBuilder)
                .Build();

            return composedConditionNode;
        }

        /// <summary>
        /// Creates a value condition node.
        /// </summary>
        /// <typeparam name="T">the operand type.</typeparam>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">
        /// The data type is not supported: {typeof(T).FullName}.
        /// </exception>
        public static IValueConditionNode CreateValueNode<T>(
            string condition, Operators condOperator, T operand)
        {
            switch (operand)
            {
                case decimal _:
                    return new ValueConditionNode(DataTypes.Decimal, condition, condOperator, operand);

                case IEnumerable<decimal> _:
                    return new ValueConditionNode(DataTypes.ArrayDecimal, condition, condOperator, operand);

                case int _:
                    return new ValueConditionNode(DataTypes.Integer, condition, condOperator, operand);

                case IEnumerable<int> _:
                    return new ValueConditionNode(DataTypes.ArrayInteger, condition, condOperator, operand);

                case bool _:
                    return new ValueConditionNode(DataTypes.Boolean, condition, condOperator, operand);

                case IEnumerable<bool> _:
                    return new ValueConditionNode(DataTypes.ArrayBoolean, condition, condOperator, operand);

                case string _:
                    return new ValueConditionNode(DataTypes.String, condition, condOperator, operand);

                case IEnumerable<string> _:
                    return new ValueConditionNode(DataTypes.ArrayString, condition, condOperator, operand);

                default:
                    throw new NotSupportedException($"The data type is not supported: {typeof(T).FullName}.");
            }
        }
    }
}