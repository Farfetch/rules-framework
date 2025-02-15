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
                parseContext.Synchronize();
                return ExpressionStatement.Create(expression, expression.BeginPosition, parseContext.GetCurrentToken().EndPosition);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.SEMICOLON))
            {
                parseContext.EnterPanicMode("Expected token ';'.", parseContext.GetNextToken());
                parseContext.Synchronize();
            }

            return ExpressionStatement.Create(expression, expression.BeginPosition, parseContext.GetCurrentToken().EndPosition);
        }
    }
}