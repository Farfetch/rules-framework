namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class UpdateExpression : Expression
    {
        public UpdateExpression(Expression ruleName, Expression contentType, Segment[] updatableAttributes)
            : base(ruleName.BeginPosition, updatableAttributes.Last().EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
            this.UpdatableAttributes = updatableAttributes;
        }

        public Expression ContentType { get; }

        public Expression RuleName { get; }

        public Segment[] UpdatableAttributes { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUpdateExpression(this);
    }
}