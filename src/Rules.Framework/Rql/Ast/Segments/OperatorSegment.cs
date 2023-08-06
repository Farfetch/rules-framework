namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql.Tokens;

    internal class OperatorSegment : Segment
    {
        public OperatorSegment(Token[] tokens)
            : base(tokens[0].BeginPosition, tokens[tokens.Length - 1].EndPosition)
        {
            this.Tokens = tokens;
        }

        public Token[] Tokens { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitOperatorSegment(this);
    }
}