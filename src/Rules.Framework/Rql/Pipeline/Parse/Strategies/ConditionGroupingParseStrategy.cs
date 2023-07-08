namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ConditionGroupingParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ConditionGroupingParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.MoveNextIfCurrentToken(TokenType.BRACKET_LEFT))
            {
                var condition = this.ParseExpressionWith<ConditionGroupingParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_RIGHT))
                {
                    parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                return new ConditionGroupingExpression(condition);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.PLACEHOLDER))
            {
                return this.ParseExpressionWith<ValueConditionParseStrategy>(parseContext);
            }

            return this.ParseExpressionWith<ComposedConditionParseStrategy>(parseContext);
        }
    }
}