using System;

namespace Rules.Framework.Core.ConditionNodes
{
    public abstract class ValueConditionNodeTemplate<T, TConditionType> : IValueConditionNode<TConditionType>
        where T : IComparable<T>
    {
        public TConditionType ConditionType { get; }

        public abstract DataTypes DataType { get; }

        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        public T Operand { get; }

        public Operators Operator { get; }
    }
}