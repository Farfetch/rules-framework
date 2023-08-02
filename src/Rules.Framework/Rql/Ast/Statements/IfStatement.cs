namespace Rules.Framework.Rql.Ast.Statements
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class IfStatement : Statement
    {
        public IfStatement(Token ifKeyword, Expression condition, Statement thenBranch, Statement elseBranch)
            : base(ifKeyword.BeginPosition, elseBranch.EndPosition)
        {
            this.IfKeyword = ifKeyword;
            this.Condition = condition;
            this.ThenBranch = thenBranch;
            this.ElseBranch = elseBranch;
        }

        public Expression Condition { get; }

        public Statement ElseBranch { get; }

        public Token IfKeyword { get; }

        public Statement ThenBranch { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitIfStatement(this);
    }
}