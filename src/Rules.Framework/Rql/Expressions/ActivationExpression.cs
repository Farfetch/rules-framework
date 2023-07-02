namespace Rules.Framework.Rql.Expressions
{
    internal class ActivationExpression : Expression
    {
        public ActivationExpression(Expression ruleName, Expression contentType)
            : base(ruleName.BeginPosition, contentType.EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
        }

        public Expression ContentType { get; }

        public Expression RuleName { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitActivationExpression(this);
    }
}