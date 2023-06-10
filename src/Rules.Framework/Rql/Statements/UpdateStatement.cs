namespace Rules.Framework.Rql.Statements
{
    using System.Linq;
    using Rules.Framework.Rql.Expressions;

    internal class UpdateStatement : Statement
    {
        public UpdateStatement(Expression ruleName, Expression contentType, Expression[] updatableAttributes)
            : base(ruleName.BeginPosition, updatableAttributes.Last().EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
            this.UpdatableAttributes = updatableAttributes;
        }

        public Expression ContentType { get; }

        public Expression RuleName { get; }

        public Expression[] UpdatableAttributes { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitUpdateStatement(this);
    }
}