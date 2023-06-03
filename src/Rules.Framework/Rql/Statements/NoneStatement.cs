namespace Rules.Framework.Rql.Statements
{
    internal class NoneStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitNoneStatement(this);
    }
}