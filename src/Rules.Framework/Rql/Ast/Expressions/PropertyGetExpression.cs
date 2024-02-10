namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class PropertyGetExpression : Expression
    {
        public PropertyGetExpression(Expression instance, Expression name)
            : base(instance.BeginPosition, name.EndPosition)
        {
            this.Instance = instance;
            this.Name = name;
        }

        public Expression Instance { get; }

        public Expression Name { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPropertyGetExpression(this);
    }
}