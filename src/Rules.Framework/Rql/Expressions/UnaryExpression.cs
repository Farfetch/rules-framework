namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class UnaryExpression : Expression
    {
        public UnaryExpression(Token @operator, Expression right)
            : base(@operator.BeginPosition, right.EndPosition)
        {
            this.Operator = @operator;
            this.Right = right;
        }

        public Token Operator { get; }

        public Expression Right { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUnaryExpression(this);
    }
}