namespace Rules.Framework.Rql.Ast.Segments
{
    internal class UpdatableAttributeSegment : Segment
    {
        public UpdatableAttributeSegment(Segment updatableAttribute, UpdatableAttributeKind kind)
            : base(updatableAttribute.BeginPosition, updatableAttribute.EndPosition)
        {
            this.UpdatableAttribute = updatableAttribute;
            this.Kind = kind;
        }

        public UpdatableAttributeKind Kind { get; }

        public Segment UpdatableAttribute { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitUpdatableAttributeSegment(this);
    }
}