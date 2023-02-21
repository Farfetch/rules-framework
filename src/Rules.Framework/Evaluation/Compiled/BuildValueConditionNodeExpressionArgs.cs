namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Rules.Framework.Core;

    internal struct BuildValueConditionNodeExpressionArgs : IEquatable<BuildValueConditionNodeExpressionArgs>
    {
        public DataTypeConfiguration DataTypeConfiguration { get; set; }

        public ParameterExpression LeftOperandVariableExpression { get; set; }

        public Operators Operator { get; set; }

        public ParameterExpression ResultVariableExpression { get; set; }

        public ParameterExpression RightOperandVariableExpression { get; set; }

        public readonly bool Equals(BuildValueConditionNodeExpressionArgs other)
            => EqualityComparer<DataTypeConfiguration>.Default.Equals(this.DataTypeConfiguration, other.DataTypeConfiguration)
                && EqualityComparer<ParameterExpression>.Default.Equals(this.LeftOperandVariableExpression, other.LeftOperandVariableExpression)
                && EqualityComparer<Operators>.Default.Equals(this.Operator, other.Operator)
                && EqualityComparer<ParameterExpression>.Default.Equals(this.ResultVariableExpression, other.ResultVariableExpression)
                && EqualityComparer<ParameterExpression>.Default.Equals(this.RightOperandVariableExpression, other.RightOperandVariableExpression);

        public override readonly bool Equals(object obj)
            => obj is BuildValueConditionNodeExpressionArgs args && this.Equals(args);

        public override readonly int GetHashCode()
        {
            int hashCode = 274727839;
            hashCode = hashCode * -1521134295 + EqualityComparer<DataTypeConfiguration>.Default.GetHashCode(this.DataTypeConfiguration);
            hashCode = hashCode * -1521134295 + EqualityComparer<ParameterExpression>.Default.GetHashCode(this.LeftOperandVariableExpression);
            hashCode = hashCode * -1521134295 + this.Operator.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ParameterExpression>.Default.GetHashCode(this.ResultVariableExpression);
            hashCode = hashCode * -1521134295 + EqualityComparer<ParameterExpression>.Default.GetHashCode(this.RightOperandVariableExpression);
            return hashCode;
        }
    }
}