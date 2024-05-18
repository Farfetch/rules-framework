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

            if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                parseContext.EnterPanicMode("Expected '{' after WITH.", parseContext.GetCurrentToken());
                return Segment.None;
            }

            var inputConditionExpression = this.ParseInputCondition(parseContext);
            if (parseContext.PanicMode)
            {
                return Segment.None;
            }

            var inputConditionExpressions = new List<Segment> { inputConditionExpression };
            while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
            {
                inputConditionExpression = this.ParseInputCondition(parseContext);
                if (parseContext.PanicMode)
                {
                    return Segment.None;
                }

                inputConditionExpressions.Add(inputConditionExpression);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BRACE_RIGHT))
            {
                parseContext.EnterPanicMode("Expected ',' or '}' after input condition.", parseContext.GetNextToken());
                return Segment.None;
            }

            return new InputConditionsSegment(inputConditionExpressions.ToArray());
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