namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class PropertyGetExpression : Expression
    {
        public PropertyGetExpression(Expression instance, Token name)
            : base(instance.BeginPosition, name.EndPosition)
        {
            this.Instance = instance;
            this.Name = name;
        }

        public Expression Instance { get; }

        public Token Name { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPropertyGetExpression(this);
    }
}