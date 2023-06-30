namespace Rules.Framework.Rql.Expressions
{
    internal class VariableExpression : Expression
    {
        public VariableExpression(Expression identifier)
            : base(identifier.BeginPosition, identifier.EndPosition)
        {
            this.Name = identifier;
        }

        public Expression Name { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitVariableExpression(this);
    }
}