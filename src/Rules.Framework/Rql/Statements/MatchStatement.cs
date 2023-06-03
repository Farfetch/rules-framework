namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;

    internal class MatchStatement : Statement
    {
        public MatchStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitMatchStatement(this);
    }
}