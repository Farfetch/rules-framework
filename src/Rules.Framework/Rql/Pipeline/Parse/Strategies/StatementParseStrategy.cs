namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class StatementParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public StatementParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                return this.ParseStatementWith<BlockParseStrategy>(parseContext);
            }

            return this.ParseStatementWith<ExpressionStatementParseStrategy>(parseContext);
        }
    }
}