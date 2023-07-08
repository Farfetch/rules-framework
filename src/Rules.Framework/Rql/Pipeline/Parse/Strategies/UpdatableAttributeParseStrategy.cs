namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class UpdatableAttributeParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public UpdatableAttributeParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.SET))
            {
                throw new InvalidOperationException("Unable to handle updatable attribute expression.");
            }

            if (parseContext.IsMatchNextToken(TokenType.ENDS))
            {
                var dateEnd = this.ParseExpressionWith<DateEndParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new UpdatableAttributeExpression(dateEnd, UpdatableAttributeKind.DateEnd);
            }

            if (parseContext.IsMatchNextToken(TokenType.PRIORITY))
            {
                var priorityOption = this.ParseExpressionWith<PriorityOptionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new UpdatableAttributeExpression(priorityOption, UpdatableAttributeKind.PriorityOption);
            }

            parseContext.EnterPanicMode("Expected updatable attribute (ENDS ON <date end>, PRIORITY NUMBER <priority value>).", parseContext.GetCurrentToken());
            return Expression.None;
        }
    }
}