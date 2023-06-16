namespace Rules.Framework.Rql.Expressions
{
    using System.Linq;
    using Rules.Framework.Rql.Tokens;

    internal class CallExpression : Expression
    {
        public CallExpression(Expression instance, Token name, Expression[] arguments)
            : base(name.BeginPosition, arguments.LastOrDefault()?.EndPosition ?? name.EndPosition)
        {
            this.Instance = instance;
            this.Name = name;
            this.Arguments = arguments;
        }

        public Expression[] Arguments { get; }

        public Expression Instance { get; }

        public Token Name { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCallExpression(this);
    }
}