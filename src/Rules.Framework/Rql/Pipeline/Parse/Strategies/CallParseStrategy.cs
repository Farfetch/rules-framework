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

            var identifier = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_LEFT))
            {
                return new VariableExpression(identifier);
            }

            if (parseContext.MoveNextIfNextToken(TokenType.BRACKET_RIGHT))
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

            if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_RIGHT))
            {
                parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return new CallExpression(Expression.None, identifier, arguments.ToArray());
        }
    }
}