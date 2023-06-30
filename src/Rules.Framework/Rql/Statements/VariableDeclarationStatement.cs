namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class VariableDeclarationStatement : Statement
    {
        public VariableDeclarationStatement(Token keyword, Expression name, Expression assignable)
            : base(keyword.BeginPosition, assignable?.EndPosition ?? name.EndPosition)
        {
            this.Keyword = keyword;
            this.Name = name;
            this.Assignable = assignable;
        }

        public Expression Assignable { get; }

        public Token Keyword { get; }

        public Expression Name { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitVariableDeclarationStatement(this);
    }
}