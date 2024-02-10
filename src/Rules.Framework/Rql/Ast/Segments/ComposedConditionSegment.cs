using Rules.Framework.Rql.Ast.Expressions;

namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    internal class ComposedConditionSegment : Segment
    {
        public ComposedConditionSegment(
            Expression logicalOperator,
            Segment[] childConditions)
            : base(childConditions.First().BeginPosition, childConditions.Last().EndPosition)
        {
            this.LogicalOperator = logicalOperator;
            this.ChildConditions = childConditions;
        }

        public Segment[] ChildConditions { get; }

        public Expression LogicalOperator { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitComposedConditionSegment(this);
    }
}