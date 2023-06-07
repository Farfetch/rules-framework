namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class DateBeginParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public DateBeginParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.STARTS))
            {
                parseContext.EnterPanicMode("Expected token 'STARTS'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.ON))
            {
                parseContext.EnterPanicMode("Expected token 'ON'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.STRING))
            {
                parseContext.EnterPanicMode("Expected begin date and time.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<DateTimeLiteralParseStrategy>(parseContext);
        }
    }
}