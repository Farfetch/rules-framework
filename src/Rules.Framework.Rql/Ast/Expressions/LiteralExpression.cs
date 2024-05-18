namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class LiteralExpression : Expression
    {
        private LiteralExpression(LiteralType type, Token token, object value)
            : base(token.BeginPosition, token.EndPosition)
        {
            this.Type = type;
            this.Token = token;
            this.Value = value;
        }

        public Token Token { get; }

        public LiteralType Type { get; }

        public object Value { get; }

        public static LiteralExpression Create(LiteralType type, Token token, object value)
            => new(type, token, value);

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitLiteralExpression(this);
    }
}