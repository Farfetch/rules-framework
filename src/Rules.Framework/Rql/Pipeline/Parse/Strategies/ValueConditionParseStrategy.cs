namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ValueConditionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ValueConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                throw new InvalidOperationException("Unable to handle value condition expression.");
            }

            var leftToken = parseContext.GetCurrentToken();
            var leftExpression = new PlaceholderExpression(leftToken);

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected binary operator.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var operatorExpression = this.ParseExpressionWith<OperatorParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.INT, TokenType.STRING, TokenType.BOOL, TokenType.DECIMAL, TokenType.IDENTIFIER))
            {
                parseContext.EnterPanicMode("Expected value for condition.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var rightExpression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new ValueConditionExpression(leftExpression, operatorExpression, rightExpression);
        }
    }
}