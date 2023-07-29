namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql;

    internal abstract class Segment : IAstElement
    {
        protected Segment(RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public static Segment None { get; } = new NoneSegment();

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public abstract T Accept<T>(ISegmentVisitor<T> visitor);
    }
}