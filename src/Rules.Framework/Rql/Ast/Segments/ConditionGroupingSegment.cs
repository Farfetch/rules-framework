namespace Rules.Framework.Rql.Ast.Segments
{
    internal class ConditionGroupingSegment : Segment
    {
        public ConditionGroupingSegment(Segment rootCondition)
            : base(rootCondition.BeginPosition, rootCondition.EndPosition)
        {
            this.RootCondition = rootCondition;
        }

        public Segment RootCondition { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitConditionGroupingSegment(this);
    }
}