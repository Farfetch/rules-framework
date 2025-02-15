namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql.Ast.Expressions;

    internal class DatesIntervalSegment : Segment
    {
        public DatesIntervalSegment(
            Expression sinceKeyword,
            Expression sinceDate,
            Expression untilKeyword,
            Expression untilDate) : base(sinceKeyword.BeginPosition, untilDate.EndPosition)
        {
            this.SinceKeyword = sinceKeyword;
            this.SinceDate = sinceDate;
            this.UntilKeyword = untilKeyword;
            this.UntilDate = untilDate;
        }

        public Expression SinceDate { get; }

        public Expression SinceKeyword { get; }

        public Expression UntilDate { get; }

        public Expression UntilKeyword { get; }

        public static DatesIntervalSegment Create(Expression sinceKeyword, Expression sinceDate, Expression untilKeyword, Expression untilDate)
            => new DatesIntervalSegment(sinceKeyword, sinceDate, untilKeyword, untilDate);

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitDatesIntervalSegment(this);
    }
}