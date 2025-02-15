namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class MatchRulesParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public MatchRulesParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.MATCH))
            {
                throw new InvalidOperationException("Unable to handle match rules expression.");
            }

            var matchKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            var cardinality = Segment.None;
            var ruleset = Segment.None;
            var matchDate = Segment.None;
            var inputConditionsExpression = Segment.None;

            _ = parseContext.MoveNext();
            cardinality = this.ParseSegmentWith<CardinalityParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return MatchExpression.Create(matchKeyword, cardinality, ruleset, matchDate, inputConditionsExpression);
            }

            ruleset = this.ParseSegmentWith<RulesetParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return MatchExpression.Create(matchKeyword, cardinality, ruleset, matchDate, inputConditionsExpression);
            }

            matchDate = this.ParseDate(parseContext);
            if (parseContext.PanicMode)
            {
                return MatchExpression.Create(matchKeyword, cardinality, ruleset, matchDate, inputConditionsExpression);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.WHEN))
            {
                inputConditionsExpression = this.ParseSegmentWith<InputConditionsParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return MatchExpression.Create(matchKeyword, cardinality, ruleset, matchDate, inputConditionsExpression);
                }
            }
            else
            {
                if (!parseContext.IsMatchNextToken(TokenType.SEMICOLON, TokenType.EOF))
                {
                    var token = parseContext.GetNextToken();
                    parseContext.EnterPanicMode($"Unrecognized token '{token.Lexeme}'.", token);
                    return MatchExpression.Create(matchKeyword, cardinality, ruleset, matchDate, inputConditionsExpression);
                }

                inputConditionsExpression = Segment.None;
            }

            return MatchExpression.Create(matchKeyword, cardinality, ruleset, matchDate, inputConditionsExpression);
        }

        private Segment ParseDate(ParseContext parseContext)
        {
            var onKeyword = Expression.None;
            var matchDate = Expression.None;
            if (!parseContext.MoveNextIfNextToken(TokenType.ON))
            {
                parseContext.EnterPanicMode("Expected token 'ON'.", parseContext.GetNextToken());
                return MatchDateSegment.Create(onKeyword, matchDate);
            }

            onKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
                return MatchDateSegment.Create(onKeyword, matchDate);
            }

            matchDate = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return MatchDateSegment.Create(onKeyword, matchDate);
            }

            if (matchDate is LiteralExpression literalExpression && literalExpression.Type != LiteralType.DateTime)
            {
                parseContext.EnterPanicMode("Expected literal of type date.", parseContext.GetCurrentToken());
            }

            return MatchDateSegment.Create(onKeyword, matchDate);
        }
    }
}