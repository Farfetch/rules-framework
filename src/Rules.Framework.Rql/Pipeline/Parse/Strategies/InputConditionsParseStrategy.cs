namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class InputConditionsParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public InputConditionsParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.WHEN))
            {
                throw new InvalidOperationException("Unable to handle input conditions expression.");
            }

            var whenKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            var beginToken = Token.None;
            var inputConditionExpressions = new List<Segment>();
            var endToken = Token.None;
            if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                parseContext.EnterPanicMode("Expected '{' after WITH.", parseContext.GetCurrentToken());
                return InputConditionsSegment.Create(whenKeyword, beginToken, inputConditionExpressions.ToArray(), endToken);
            }

            beginToken = parseContext.GetCurrentToken();
            var inputConditionExpression = this.ParseInputCondition(parseContext);
            inputConditionExpressions.Add(inputConditionExpression);
            if (parseContext.PanicMode)
            {
                return InputConditionsSegment.Create(whenKeyword, beginToken, inputConditionExpressions.ToArray(), endToken);
            }

            while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
            {
                inputConditionExpression = this.ParseInputCondition(parseContext);
                inputConditionExpressions.Add(inputConditionExpression);
                if (parseContext.PanicMode)
                {
                    return InputConditionsSegment.Create(whenKeyword, beginToken, inputConditionExpressions.ToArray(), endToken);
                }
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BRACE_RIGHT))
            {
                parseContext.EnterPanicMode("Expected ',' or '}' after input condition.", parseContext.GetNextToken());
            }

            endToken = parseContext.GetCurrentToken();
            return InputConditionsSegment.Create(whenKeyword, beginToken, inputConditionExpressions.ToArray(), endToken);
        }

        private Segment ParseInputCondition(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfNextToken(TokenType.PLACEHOLDER))
            {
                return this.ParseSegmentWith<InputConditionParseStrategy>(parseContext);
            }

            parseContext.EnterPanicMode("Expected placeholder (@<placeholder name>) for condition.", parseContext.GetNextToken());
            return Segment.None;
        }
    }
}