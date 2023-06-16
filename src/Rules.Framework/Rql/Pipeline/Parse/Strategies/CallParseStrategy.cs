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
            if (!parseContext.IsMatchCurrentToken(TokenType.IDENTIFIER))
            {
                throw new InvalidOperationException("Unable to handle call expression.");
            }

            var call = this.ParseCall(parseContext);
            while (parseContext.MoveNextIfNextToken(TokenType.DOT))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.IDENTIFIER))
                {
                    parseContext.EnterPanicMode("Expected identifier after '.'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var chainedCall = this.ParseCall(parseContext);
                if (chainedCall is VariableExpression variableExpression)
                {
                    call = new PropertyGetExpression(call, variableExpression.Token);
                    continue;
                }

                if (chainedCall is CallExpression callExpression)
                {
                    call = new CallExpression(call, callExpression.Name, callExpression.Arguments);
                    continue;
                }
            }

            return call;
        }

        private Expression ParseCall(ParseContext parseContext)
        {
            var identifier = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.PARENTHESIS_LEFT))
            {
                return new VariableExpression(identifier);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.PARENTHESIS_RIGHT))
            {
                return new CallExpression(Expression.None, identifier, Array.Empty<Expression>());
            }

            _ = parseContext.MoveNext();
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

            return new CallExpression(Expression.None, identifier, arguments.ToArray());
        }
    }
}