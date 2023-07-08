namespace Rules.Framework.Rql.Ast.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class OperatorExpression : Expression
    {
        public OperatorExpression(Token token)
            : base(token.BeginPosition, token.EndPosition)
        {
            this.Token = token;
        }

        public Token Token { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitOperatorExpression(this);
    }
}