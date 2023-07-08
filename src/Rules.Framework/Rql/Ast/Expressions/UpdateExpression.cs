namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Linq;

    internal class UpdateExpression : Expression
    {
        public UpdateExpression(Expression ruleName, Expression contentType, Expression[] updatableAttributes)
            : base(ruleName.BeginPosition, updatableAttributes.Last().EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
            this.UpdatableAttributes = updatableAttributes;
        }

        public Expression ContentType { get; }

        public Expression RuleName { get; }

        public Expression[] UpdatableAttributes { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUpdateExpression(this);
    }
}