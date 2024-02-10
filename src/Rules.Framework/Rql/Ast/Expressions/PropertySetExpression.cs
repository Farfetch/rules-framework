namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class PropertySetExpression : Expression
    {
        public PropertySetExpression(Expression instance, Expression name, Token assign, Expression value)
            : base(instance.EndPosition, value.EndPosition)
        {
            this.Instance = instance;
            this.Name = name;
            this.Assign = assign;
            this.Value = value;
        }

        public Token Assign { get; }

        public Expression Instance { get; }

        public Expression Name { get; }

        public Expression Value { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPropertySetExpression(this);
    }
}