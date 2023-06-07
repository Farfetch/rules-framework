namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ConditionGroupingParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ConditionGroupingParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfNextToken(TokenType.PARENTHESIS_LEFT))
            {
                var composedCondition = this.ParseExpressionWith<ComposedConditionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                if (!parseContext.MoveNextIfNextToken(TokenType.PARENTHESIS_RIGHT))
                {
                    parseContext.EnterPanicMode("Expect token ')'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                return new ConditionGroupingExpression(composedCondition);
            }

            return this.ParseExpressionWith<ComposedConditionParseStrategy>(parseContext);
        }
    }
}