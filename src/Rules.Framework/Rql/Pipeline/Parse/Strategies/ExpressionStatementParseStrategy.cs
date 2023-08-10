namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class ExpressionStatementParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public ExpressionStatementParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            var expression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.SEMICOLON))
            {
                parseContext.EnterPanicMode("Expected token ';'.", parseContext.GetNextToken());
                return Statement.None;
            }

            return new ExpressionStatement(expression);
        }
    }
}