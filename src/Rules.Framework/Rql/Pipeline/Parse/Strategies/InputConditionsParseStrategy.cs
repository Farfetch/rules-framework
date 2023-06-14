namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class InputConditionsParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public InputConditionsParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.WITH))
            {
                throw new InvalidOperationException("Unable to handle input conditions expression.");
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                parseContext.EnterPanicMode("Expected '{' after WITH.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var inputConditionExpression = this.ParseInputCondition(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var inputConditionExpressions = new List<Expression> { inputConditionExpression };
            while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
            {
                inputConditionExpression = this.ParseInputCondition(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                inputConditionExpressions.Add(inputConditionExpression);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BRACE_RIGHT))
            {
                parseContext.EnterPanicMode("Expected ',' or '}' after input condition.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return new InputConditionsExpression(inputConditionExpressions.ToArray());
        }

        private Expression ParseInputCondition(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfNextToken(TokenType.PLACEHOLDER))
            {
                return this.ParseExpressionWith<InputConditionParseStrategy>(parseContext);
            }

            parseContext.EnterPanicMode("Expected placeholder (@<placeholder name>) for condition.", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}