namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    internal class CallExpression : Expression
    {
        public CallExpression(Expression instance, Expression name, Expression[] arguments)
            : base(name.BeginPosition, arguments.LastOrDefault()?.EndPosition ?? name.EndPosition)
        {
            this.Instance = instance;
            this.Name = name;
            this.Arguments = arguments;
        }

        public Expression[] Arguments { get; }

        public Expression Instance { get; }

        public Expression Name { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCallExpression(this);
    }
}