namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
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