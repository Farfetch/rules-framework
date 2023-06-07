namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class PriorityOptionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public PriorityOptionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.SET))
            {
                parseContext.EnterPanicMode("Expected token 'SET'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.PRIORITY))
            {
                parseContext.EnterPanicMode("Expected token 'PRIORITY'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.TOP, TokenType.BOTTOM))
            {
                return ParseTopOrBottom(parseContext);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.RULE))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.NAME))
                {
                    parseContext.EnterPanicMode("Expected token 'NAME'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                return ParseRuleName(parseContext);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.NUMBER))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.INT))
                {
                    parseContext.EnterPanicMode("Expected token 'NUMBER'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                return ParsePriorityNumber(parseContext);
            }

            parseContext.EnterPanicMode("Expect one priority option (TOP, BOTTOM, RULE NAME <name>, or NUMBER <priority value>.", parseContext.GetCurrentToken());
            return Expression.None;
        }

        private Expression ParsePriorityNumber(ParseContext parseContext)
        {
            var keyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            var priorityValue = this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new PriorityOptionExpression(keyword, priorityValue);
        }

        private Expression ParseRuleName(ParseContext parseContext)
        {
            var keyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            var ruleName = this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new PriorityOptionExpression(keyword, ruleName);
        }

        private Expression ParseTopOrBottom(ParseContext parseContext)
        {
            var keyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new PriorityOptionExpression(keyword, argument: null);
        }
    }
}