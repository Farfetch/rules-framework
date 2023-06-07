namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class PlaceholderExpression : Expression
    {
        public PlaceholderExpression(Token token)
            : base(token.BeginPosition, token.EndPosition)
        {
            this.Token = token;
        }

        public Token Token { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPlaceholderExpression(this);
    }
}