namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class NewArrayExpression : Expression
    {
        private NewArrayExpression(Token array, Token initializerBeginToken, Expression size, Expression[] values, Token initializerEndToken)
            : base(array.EndPosition, initializerEndToken.EndPosition)
        {
            this.Array = array;
            this.InitializerBeginToken = initializerBeginToken;
            this.Size = size;
            this.Values = values;
            this.InitializerEndToken = initializerEndToken;
        }

        public Token Array { get; }

        public Token InitializerBeginToken { get; }

        public Token InitializerEndToken { get; }

        public Expression Size { get; }

        public Expression[] Values { get; }

        public static NewArrayExpression Create(Token array, Token initializerBeginToken, Expression size, Expression[] values, Token initializerEndToken)
            => new(array, initializerBeginToken, size, values, initializerEndToken);

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitNewArrayExpression(this);
    }
}