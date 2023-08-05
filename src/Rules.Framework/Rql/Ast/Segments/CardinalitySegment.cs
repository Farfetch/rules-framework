namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql.Ast.Expressions;

    internal class CardinalitySegment : Segment
    {
        public CardinalitySegment(Expression cardinalityKeyword, Expression ruleKeyword)
            : base(cardinalityKeyword.BeginPosition, ruleKeyword.EndPosition)
        {
            this.CardinalityKeyword = cardinalityKeyword;
            this.RuleKeyword = ruleKeyword;
        }

        public Expression CardinalityKeyword { get; }

        public Expression RuleKeyword { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitCardinalitySegment(this);
    }
}