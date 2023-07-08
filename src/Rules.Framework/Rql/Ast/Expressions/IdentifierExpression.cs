namespace Rules.Framework.Rql.Ast.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class IdentifierExpression : Expression
    {
        public IdentifierExpression(Token identifier)
            : base(identifier.BeginPosition, identifier.EndPosition)
        {
            this.Identifier = identifier;
        }

        public Token Identifier { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIdentifierExpression(this);
    }
}