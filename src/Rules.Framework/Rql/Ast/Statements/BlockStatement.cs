namespace Rules.Framework.Rql.Ast.Statements
{
    using Rules.Framework.Rql.Tokens;

    internal class BlockStatement : Statement
    {
        public BlockStatement(Token beginBrace, Statement[] statements, Token endBrace)
            : base(beginBrace.BeginPosition, endBrace.EndPosition)
        {
            this.BeginBrace = beginBrace;
            this.Statements = statements;
            this.EndBrace = endBrace;
        }

        public Token BeginBrace { get; }

        public Token EndBrace { get; }

        public Statement[] Statements { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitBlockStatement(this);
    }
}