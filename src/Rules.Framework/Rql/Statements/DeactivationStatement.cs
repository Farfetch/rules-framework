namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;

    internal class DeactivationStatement : Statement
    {
        public DeactivationStatement(Expression ruleName, Expression contentType)
            : base(ruleName.BeginPosition, contentType.EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
        }

        public Expression ContentType { get; }

        public Expression RuleName { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitDeactivationStatement(this);
    }
}