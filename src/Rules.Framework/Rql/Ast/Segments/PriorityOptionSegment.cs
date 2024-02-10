namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;

    [ExcludeFromCodeCoverage]
    internal class PriorityOptionSegment : Segment
    {
        public PriorityOptionSegment(
            Expression priorityOption,
            Expression argument)
            : base(priorityOption.BeginPosition, argument?.EndPosition ?? priorityOption.EndPosition)
        {
            this.PriorityOption = priorityOption;
            this.Argument = argument;
        }

        public Expression Argument { get; }

        public Expression PriorityOption { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitPriorityOptionSegment(this);
    }
}