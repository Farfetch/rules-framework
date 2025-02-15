namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class SearchRulesParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public SearchRulesParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.SEARCH))
            {
                throw new InvalidOperationException("Unable to handle search rules expression.");
            }

            var searchKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            var rulesKeyword = Expression.None;
            var ruleset = Segment.None;
            var datesInterval = Segment.None;
            var inputConditionsExpression = Segment.None;

            if (!parseContext.MoveNextIfNextToken(TokenType.RULES))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.RULES)}'.", parseContext.GetCurrentToken());
                return SearchExpression.Create(searchKeyword, rulesKeyword, ruleset, datesInterval, inputConditionsExpression);
            }

            rulesKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            ruleset = this.ParseSegmentWith<RulesetParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return SearchExpression.Create(searchKeyword, rulesKeyword, ruleset, datesInterval, inputConditionsExpression);
            }

            datesInterval = this.ParseDatesInterval(parseContext);
            if (parseContext.PanicMode)
            {
                return SearchExpression.Create(searchKeyword, rulesKeyword, ruleset, datesInterval, inputConditionsExpression);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.WHEN))
            {
                inputConditionsExpression = this.ParseSegmentWith<InputConditionsParseStrategy>(parseContext);
            }
            else
            {
                if (!parseContext.IsMatchNextToken(TokenType.SEMICOLON, TokenType.EOF))
                {
                    var token = parseContext.GetNextToken();
                    parseContext.EnterPanicMode($"Unrecognized token '{token.Lexeme}'.", token);
                }
            }

            return SearchExpression.Create(searchKeyword, rulesKeyword, ruleset, datesInterval, inputConditionsExpression);
        }

        private Segment ParseDatesInterval(ParseContext parseContext)
        {
            var sinceKeyword = Expression.None;
            var sinceDate = Expression.None;
            var untilKeyword = Expression.None;
            var untilDate = Expression.None;
            if (!parseContext.MoveNextIfNextToken(TokenType.SINCE))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.SINCE)}'.", parseContext.GetNextToken());
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            sinceKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            sinceDate = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            if (sinceDate is LiteralExpression sinceDateAsLiteralExpression && sinceDateAsLiteralExpression.Type != LiteralType.DateTime)
            {
                parseContext.EnterPanicMode("Expected literal of type date.", sinceDateAsLiteralExpression.Token);
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.UNTIL))
            {
                parseContext.EnterPanicMode($"Expected token '{nameof(TokenType.UNTIL)}'.", parseContext.GetNextToken());
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            untilKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            untilDate = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
            }

            if (untilDate is LiteralExpression untilDateAsLiteralExpression && untilDateAsLiteralExpression.Type != LiteralType.DateTime)
            {
                parseContext.EnterPanicMode("Expected literal of type date.", untilDateAsLiteralExpression.Token);
            }

            return DatesIntervalSegment.Create(sinceKeyword, sinceDate, untilKeyword, untilDate);
        }
    }
}