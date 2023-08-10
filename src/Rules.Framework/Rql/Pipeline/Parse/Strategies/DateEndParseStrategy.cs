namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class DateEndParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public DateEndParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.ENDS))
            {
                throw new InvalidOperationException("Unable to handle date begin expression.");
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.ON))
            {
                parseContext.EnterPanicMode("Expected token 'ON'.", parseContext.GetNextToken());
                return Expression.None;
            }

            _ = parseContext.MoveNext();
            return this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
        }
    }
}