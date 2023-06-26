namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class IndexerParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public IndexerParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.IDENTIFIER))
            {
                throw new InvalidOperationException("Unable to handle indexer expression.");
            }

            var expression = this.ParseIndexer(parseContext);
            while (parseContext.MoveNextIfNextToken(TokenType.DOT))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.IDENTIFIER))
                {
                    parseContext.EnterPanicMode("Expected identifier after '.'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var chainedCall = this.ParseIndexer(parseContext);
                if (chainedCall is VariableExpression variableExpression)
                {
                    expression = new PropertyGetExpression(expression, variableExpression.Token);
                    continue;
                }

                if (chainedCall is CallExpression callExpression)
                {
                    expression = new CallExpression(expression, callExpression.Name, callExpression.Arguments);
                    continue;
                }

                if (chainedCall is IndexerExpression indexerExpression)
                {
                    expression = new IndexerExpression(expression, indexerExpression.IndexLeftDelimeter, indexerExpression.Index, indexerExpression.IndexRightDelimeter);
                    continue;
                }
            }

            return expression;
        }

        private Expression ParseIndexer(ParseContext parseContext)
        {
            var call = this.ParseExpressionWith<CallParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_LEFT))
            {
                var indexDelimeterLeft = parseContext.GetCurrentToken();
                if (!parseContext.MoveNextIfNextToken(TokenType.INT))
                {
                    parseContext.EnterPanicMode("Expected integer literal token as index.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var index = this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                if (!parseContext.MoveNextIfNextToken(TokenType.STRAIGHT_BRACKET_RIGHT))
                {
                    parseContext.EnterPanicMode("Expected token ']'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var indexDelimeterRight = parseContext.GetCurrentToken();
                return new IndexerExpression(call, indexDelimeterLeft, index, indexDelimeterRight);
            }

            return call;
        }
    }
}