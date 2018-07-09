using System;

namespace Rules.Framework.Core.ConditionNodes
{
    public abstract class ValueConditionNodeTemplate<T, TConditionType> : IValueConditionNode<TConditionType>
        where T : IComparable<T>
    {
        public ValueConditionNodeTemplate(TConditionType conditionType, Operators @operator, T operand)
        {
            this.ConditionType = conditionType;
            this.Operand = operand;
            this.Operator = @operator;
        }

        public TConditionType ConditionType { get; }

        public abstract DataTypes DataType { get; }

        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        public T Operand { get; }

        public Operators Operator { get; }
    }
}