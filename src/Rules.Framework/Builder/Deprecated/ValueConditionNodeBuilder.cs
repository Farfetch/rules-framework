namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class ValueConditionNodeBuilder<TConditionType> : IValueConditionNodeBuilder<TConditionType>
    {
        private readonly TConditionType conditionType;

        public ValueConditionNodeBuilder(TConditionType conditionType)
        {
            this.conditionType = conditionType;
        }

        public IValueConditionNodeBuilder<TConditionType, T> OfDataType<T>()
            => new ValueConditionNodeBuilder<TConditionType, T>(this.conditionType);
    }

    internal sealed class ValueConditionNodeBuilder<TConditionType, TDataType> : IValueConditionNodeBuilder<TConditionType, TDataType>
    {
        private readonly TConditionType conditionType;
        private Operators comparisonOperator;
        private object internalId;
        private object operand;

        public ValueConditionNodeBuilder(TConditionType conditionType)
        {
            this.conditionType = conditionType;
        }

        public ValueConditionNodeBuilder(TConditionType conditionType, Operators comparisonOperator, object operand)
        {
            this.conditionType = conditionType;
            this.comparisonOperator = comparisonOperator;
            this.operand = operand;
        }

        public IValueConditionNode<TConditionType> Build()
        {
            switch (this.operand)
            {
                case decimal _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Decimal, this.conditionType, this.comparisonOperator, this.operand);

                case IEnumerable<decimal> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayDecimal, this.conditionType, this.comparisonOperator, this.operand);

                case int _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Integer, this.conditionType, this.comparisonOperator, this.operand);

                case IEnumerable<int> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayInteger, this.conditionType, this.comparisonOperator, this.operand);

                case bool _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Boolean, this.conditionType, this.comparisonOperator, this.operand);

                case IEnumerable<bool> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayBoolean, this.conditionType, this.comparisonOperator, this.operand);

                case string _:
                    return new ValueConditionNode<TConditionType>(DataTypes.String, this.conditionType, this.comparisonOperator, this.operand);

                case IEnumerable<string> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.ArrayString, this.conditionType, this.comparisonOperator, this.operand);

                default:
                    throw new NotSupportedException($"The data type is not supported: {typeof(TDataType).FullName}.");
            }
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> SetOperand(TDataType value)
        {
            this.operand = value;

            return this;
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> SetOperand(IEnumerable<TDataType> value)
        {
            this.operand = value;

            return this;
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> WithComparisonOperator(Operators comparisonOperator)
        {
            this.comparisonOperator = comparisonOperator;

            return this;
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> WithInternalId(object internalId)
        {
            this.internalId = internalId;

            return this;
        }
    }
}