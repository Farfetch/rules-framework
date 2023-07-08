namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ComposedConditionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ComposedConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var conditionGrouping = this.ParseExpressionWith<ConditionGroupingParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.IsMatchNextToken(TokenType.AND, TokenType.OR))
            {
                parseContext.EnterPanicMode("Expected logical operator ('AND' or 'OR').", parseContext.GetCurrentToken());
                return Expression.None;
            }

            TokenType logicalOperatorTokenType = TokenType.None;
            Expression logicalOperator = null;
            var childConditions = new List<Expression> { conditionGrouping };
            while (parseContext.MoveNextIfNextToken(TokenType.AND, TokenType.OR))
            {
                if (logicalOperatorTokenType == TokenType.None)
                {
                    logicalOperatorTokenType = parseContext.GetCurrentToken().Type;
                    logicalOperator = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }
                }
                else if (!parseContext.IsMatchCurrentToken(logicalOperatorTokenType))
                {
                    parseContext.EnterPanicMode("Mixup of logical operators ('AND' + 'OR') under the same condition grouping is not supported.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                if (!parseContext.MoveNext())
                {
                    parseContext.EnterPanicMode("Expected condition after logical operator .", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var nextCondition = this.ParseExpressionWith<ConditionGroupingParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                childConditions.Add(nextCondition);
            }

            return new ComposedConditionExpression(logicalOperator, childConditions.ToArray());
        }
    }
}