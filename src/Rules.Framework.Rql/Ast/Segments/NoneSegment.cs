namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class NoneSegment : Segment
    {
        public NoneSegment()
            : base(RqlSourcePosition.Empty, RqlSourcePosition.Empty)
        {
        }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitNoneSegment(this);
    }
}