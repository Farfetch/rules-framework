namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal struct BuildConditionExpressionArgs : IEquatable<BuildConditionExpressionArgs>
    {
        public DataTypeConfiguration DataTypeConfiguration { get; set; }

        public Expression LeftHandOperand { get; set; }

        public Expression RightHandOperand { get; set; }

        public override readonly bool Equals(object obj)
            => obj is BuildConditionExpressionArgs args && this.Equals(args);

        public readonly bool Equals(BuildConditionExpressionArgs other)
            => EqualityComparer<DataTypeConfiguration>.Default.Equals(this.DataTypeConfiguration, other.DataTypeConfiguration)
                && EqualityComparer<Expression>.Default.Equals(this.LeftHandOperand, other.LeftHandOperand)
                && EqualityComparer<Expression>.Default.Equals(this.RightHandOperand, other.RightHandOperand);

        public override readonly int GetHashCode()
            => HashCode.Combine(this.DataTypeConfiguration, this.LeftHandOperand, this.RightHandOperand);
    }
}