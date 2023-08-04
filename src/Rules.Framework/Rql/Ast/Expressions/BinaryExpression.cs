namespace Rules.Framework.Rql.Ast.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class BinaryExpression : Expression
    {
        public BinaryExpression(Expression leftExpression, Token operatorToken, Expression rightExpression)
            : base(leftExpression.BeginPosition, rightExpression.EndPosition)
        {
            this.LeftExpression = leftExpression;
            this.OperatorToken = operatorToken;
            this.RightExpression = rightExpression;
        }

        public Expression LeftExpression { get; }

        public Token OperatorToken { get; }

        public Expression RightExpression { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBinaryExpression(this);
    }
}