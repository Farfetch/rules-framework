namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class CreateRuleParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public CreateRuleParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.CREATE))
            {
                throw new InvalidOperationException("Unable to handle create rule statement.");
            }

            var ruleName = this.ParseExpressionWith<RuleNameParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            var ruleContent = ParseContent(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            var dateBegin = this.ParseExpressionWith<DateBeginParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            (var dateEnd, var condition, var priorityOption) = this.ParseOptionals(parseContext);

            if (parseContext.IsMatchNextToken(TokenType.SEMICOLON))
            {
                _ = parseContext.MoveNext();
            }

            return new CreateStatement(ruleName, contentType, ruleContent, dateBegin, dateEnd, condition, priorityOption);
        }

        private Expression ParseContent(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.WITH))
            {
                parseContext.EnterPanicMode("Expect token 'WITH'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.CONTENT))
            {
                parseContext.EnterPanicMode("Expect token 'CONTENT'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.STRING))
            {
                parseContext.EnterPanicMode("Expect rule content under string format.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<DefaultLiteralParseStrategy>(parseContext);
        }

        private (Expression, Expression, Expression) ParseOptionals(ParseContext parseContext)
        {
            Expression dateEnd = null;
            if (parseContext.IsMatchNextToken(TokenType.ENDS))
            {
                dateEnd = this.ParseExpressionWith<DateEndParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return (Expression.None, Expression.None, Expression.None);
                }
            }

            Expression condition = null;
            if (parseContext.IsMatchNextToken(TokenType.APPLY))
            {
                condition = this.ParseExpressionWith<ConditionsDefinitionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return (Expression.None, Expression.None, Expression.None);
                }
            }

            Expression priorityOption = null;
            if (parseContext.MoveNextIfNextToken(TokenType.SET))
            {
                priorityOption = this.ParseExpressionWith<PriorityOptionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return (Expression.None, Expression.None, Expression.None);
                }
            }

            return (dateEnd, condition, priorityOption);
        }
    }
}