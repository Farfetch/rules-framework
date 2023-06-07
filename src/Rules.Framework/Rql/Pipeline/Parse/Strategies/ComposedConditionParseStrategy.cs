namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ComposedConditionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ComposedConditionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchNextToken(TokenType.PARENTHESIS_LEFT))
            {
                var conditionGrouping = this.ParseExpressionWith<ConditionGroupingParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                if (parseContext.MoveNextIfNextToken(TokenType.AND, TokenType.OR))
                {
                    var logicalOperatorTokenType = parseContext.GetCurrentToken().Type;
                    var logicalOperator = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    var childConditions = new List<Expression> { conditionGrouping };
                    while (parseContext.IsMatchCurrentToken(TokenType.AND, TokenType.OR))
                    {
                        if (!parseContext.IsMatchCurrentToken(logicalOperatorTokenType))
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
                        _ = parseContext.MoveNext();
                    }

                    return new ComposedConditionExpression(logicalOperator, childConditions.ToArray());
                }

                return conditionGrouping;
            }

            return this.ParseExpressionWith<ValueConditionParseStrategy>(parseContext);
        }
    }
}