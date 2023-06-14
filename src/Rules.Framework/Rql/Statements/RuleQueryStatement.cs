namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;

    internal class RuleQueryStatement : Statement
    {
        public RuleQueryStatement(Expression query)
            : base(query.BeginPosition, query.EndPosition)
        {
            this.Query = query;
        }

        public Expression Query { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitQueryStatement(this);
    }
}