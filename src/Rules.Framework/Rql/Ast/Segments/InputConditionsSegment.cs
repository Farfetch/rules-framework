namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Linq;

    internal class InputConditionsSegment : Segment
    {
        public InputConditionsSegment(Segment[] inputConditions)
            : base(inputConditions.FirstOrDefault()?.BeginPosition ?? RqlSourcePosition.Empty, inputConditions.LastOrDefault()?.EndPosition ?? RqlSourcePosition.Empty)
        {
            this.InputConditions = inputConditions;
        }

        public Segment[] InputConditions { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitInputConditionsSegment(this);
    }
}