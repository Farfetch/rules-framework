namespace Rules.Framework.Rql.Expressions
{
    using System.Linq;
    using Rules.Framework.Rql.Tokens;

    internal class CallExpression : Expression
    {
        public CallExpression(Token identifier, Expression[] arguments)
            : base(identifier.BeginPosition, arguments.LastOrDefault()?.EndPosition ?? identifier.EndPosition)
        {
            this.Identifier = identifier;
            this.Arguments = arguments;
        }

        public Expression[] Arguments { get; }

        public Token Identifier { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCallExpression(this);
    }
}