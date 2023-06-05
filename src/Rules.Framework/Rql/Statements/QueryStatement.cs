namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;

    internal class QueryStatement : Statement
    {
        public QueryStatement(Expression query)
        {
            this.Query = query;
        }

        public Expression Query { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitQueryStatement(this);
    }
}