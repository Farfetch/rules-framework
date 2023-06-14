namespace Rules.Framework.Rql.Statements
{
    internal class RuleDefinitionStatement : Statement
    {
        public RuleDefinitionStatement(Statement definition)
            : base(definition.BeginPosition, definition.EndPosition)
        {
            this.Definition = definition;
        }

        public Statement Definition { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitDefinitionStatement(this);
    }
}