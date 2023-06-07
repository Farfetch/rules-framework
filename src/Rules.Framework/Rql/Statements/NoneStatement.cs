namespace Rules.Framework.Rql.Statements
{
    internal class NoneStatement : Statement
    {
        public NoneStatement()
            : base(RqlSourcePosition.Empty, RqlSourcePosition.Empty)
        {
        }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitNoneStatement(this);
    }
}