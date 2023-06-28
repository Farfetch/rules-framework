namespace Rules.Framework.Rql.Expressions
{
    internal class DeactivationExpression : Expression
    {
        public DeactivationExpression(Expression ruleName, Expression contentType)
            : base(ruleName.BeginPosition, contentType.EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
        }

        public Expression ContentType { get; }

        public Expression RuleName { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitDeactivationExpression(this);
    }
}