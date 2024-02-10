namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;

    [ExcludeFromCodeCoverage]
    internal class DateEndSegment : Segment
    {
        public DateEndSegment(Expression dateEnd)
            : base(dateEnd.BeginPosition, dateEnd.EndPosition)
        {
            this.DateEnd = dateEnd;
        }

        public Expression DateEnd { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitDateEndSegment(this);
    }
}