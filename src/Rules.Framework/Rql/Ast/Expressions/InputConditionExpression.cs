namespace Rules.Framework.Rql.Ast.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class InputConditionExpression : Expression
    {
        public InputConditionExpression(Expression left, Token @operator, Expression right)
            : base(left.BeginPosition, right.EndPosition)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        public Expression Left { get; }

        public Token Operator { get; }

        public Expression Right { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitInputConditionExpression(this);
    }
}