namespace Rules.Framework.Rql.Ast.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class LiteralExpression : Expression
    {
        public LiteralExpression(LiteralType type, Token token, object value)
            : base(token.BeginPosition, token.EndPosition)
        {
            this.Type = type;
            this.Token = token;
            this.Value = value;
        }

        public Token Token { get; }

        public LiteralType Type { get; }

        public object Value { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitLiteralExpression(this);
    }
}