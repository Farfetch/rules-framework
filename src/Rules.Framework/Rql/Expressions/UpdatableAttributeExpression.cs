namespace Rules.Framework.Rql.Expressions
{
    internal class UpdatableAttributeExpression : Expression
    {
        public UpdatableAttributeExpression(Expression updatableAttribute, UpdatableAttributeKind kind)
            : base(updatableAttribute.BeginPosition, updatableAttribute.EndPosition)
        {
            this.UpdatableAttribute = updatableAttribute;
            this.Kind = kind;
        }

        public UpdatableAttributeKind Kind { get; }

        public Expression UpdatableAttribute { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUpdatableAttributeExpression(this);
    }
}