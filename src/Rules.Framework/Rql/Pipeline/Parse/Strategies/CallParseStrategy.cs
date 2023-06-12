namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class CallParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public CallParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.IDENTIFIER) || !parseContext.IsMatchNextToken(TokenType.PARENTHESIS_LEFT))
            {
                throw new InvalidOperationException("Unable to handle call expression.");
            }

            var identifier = parseContext.GetCurrentToken();
            _ = parseContext.MoveNext();

            if (parseContext.MoveNextIfNextToken(TokenType.PARENTHESIS_RIGHT))
            {
                return new CallExpression(identifier, Array.Empty<Expression>());
            }

            var argument = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var arguments = new List<Expression> { argument };
            while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
            {
                argument = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                arguments.Add(argument);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.PARENTHESIS_RIGHT))
            {
                parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return new CallExpression(identifier, arguments.ToArray());
        }
    }
}