namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;

    internal class ProgrammableSubLanguageStatement : Statement
    {
        public ProgrammableSubLanguageStatement(Expression expression)
            : base(expression.BeginPosition, expression.EndPosition)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitProgrammableSubLanguageStatement(this);
    }
}