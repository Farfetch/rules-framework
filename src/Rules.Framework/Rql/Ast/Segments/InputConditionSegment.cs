using Rules.Framework.Rql.Ast.Expressions;

namespace Rules.Framework.Rql.Ast.Segments
{
    using Rules.Framework.Rql.Tokens;

    internal class InputConditionSegment : Segment
    {
        public InputConditionSegment(Expression left, Token @operator, Expression right)
            : base(left.BeginPosition, right.EndPosition)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        public Expression Left { get; }

        public Token Operator { get; }

        public Expression Right { get; }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitInputConditionSegment(this);
    }
}