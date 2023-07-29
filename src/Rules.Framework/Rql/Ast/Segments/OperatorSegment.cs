namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql.Tokens;

    internal class OperatorSegment : Segment
    {
        public OperatorSegment(Token token)
            : base(token.BeginPosition, token.EndPosition)
        {
            this.Token = token;
        }

        public Token Token { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitOperatorSegment(this);
    }
}