namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class AssignmentExpression : Expression
    {
        public AssignmentExpression(Token left, Token assign, Expression right)
            : base(left.BeginPosition, right.EndPosition)
        {
            this.Left = left;
            this.Assign = assign;
            this.Right = right;
        }

        public Token Assign { get; }

        public Token Left { get; }

        public Expression Right { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitAssignExpression(this);
    }
}