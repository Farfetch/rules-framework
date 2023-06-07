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
            if (parseContext.MoveNext() && parseContext.MoveNextIfCurrentToken(TokenType.WITH))
            {
                if (!parseContext.MoveNextIfCurrentToken(TokenType.BRACE_LEFT))
                {
                    parseContext.EnterPanicMode("Expect '{' after WITH.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var inputConditionExpressions = new List<Expression>();
                while (true)
                {
                    var inputConditionExpression = this.ParseExpressionWith<InputConditionParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    inputConditionExpressions.Add(inputConditionExpression);

                    if (parseContext.MoveNext())
                    {
                        if (parseContext.IsMatchCurrentToken(TokenType.COMMA))
                        {
                            _ = parseContext.MoveNext();
                            continue;
                        }

                        if (parseContext.IsMatchCurrentToken(TokenType.BRACE_RIGHT))
                        {
                            _ = parseContext.MoveNext();
                            break;
                        }
                    }

                    parseContext.EnterPanicMode("Expect ',' or '}' after input condition.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                return new InputConditionsExpression(inputConditionExpressions.ToArray());
            }

            return new InputConditionsExpression(Array.Empty<Expression>());
        }
    }
}