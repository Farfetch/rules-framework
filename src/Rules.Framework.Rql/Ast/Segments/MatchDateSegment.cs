namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql.Ast.Expressions;

    internal class MatchDateSegment : Segment
    {
        public MatchDateSegment(Expression onKeyword, Expression matchDate)
            : base(onKeyword.BeginPosition, matchDate.EndPosition)
        {
            this.OnKeyword = onKeyword;
            this.MatchDate = matchDate;
        }

        public Expression MatchDate { get; }

        public Expression OnKeyword { get; }

        public static MatchDateSegment Create(Expression onKeyword, Expression matchDate)
            => new MatchDateSegment(onKeyword, matchDate);

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitMatchDateSegment(this);
    }
}