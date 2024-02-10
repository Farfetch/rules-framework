namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;

    [ExcludeFromCodeCoverage]
    internal class ValueConditionSegment : Segment
    {
        public ValueConditionSegment(
            Expression left,
            Segment @operator,
            Expression right)
            : base(left.BeginPosition, right.EndPosition)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        public Expression Left { get; }

        public Segment Operator { get; }

        public Expression Right { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitValueConditionSegment(this);
    }
}