namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class CreateRuleParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public CreateRuleParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.CREATE))
            {
                throw new InvalidOperationException("Unable to handle create rule statement.");
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.RULE))
            {
                parseContext.EnterPanicMode("Expected token 'RULE'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var ruleName = this.ParseExpressionWith<RuleNameParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.FOR))
            {
                parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var ruleContent = ParseContent(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BEGINS))
            {
                parseContext.EnterPanicMode("Expected token 'STARTS'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var dateBegin = this.ParseExpressionWith<DateBeginParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            (var dateEnd, var condition, var priorityOption) = this.ParseOptionals(parseContext);

            return new CreateExpression(ruleName, contentType, ruleContent, dateBegin, dateEnd, condition, priorityOption);
        }

        private Expression ParseContent(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfNextToken(TokenType.WITH))
            {
                parseContext.EnterPanicMode("Expected token 'WITH'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.CONTENT))
            {
                parseContext.EnterPanicMode("Expected token 'CONTENT'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.INT, TokenType.DECIMAL, TokenType.STRING, TokenType.BOOL, TokenType.IDENTIFIER))
            {
                parseContext.EnterPanicMode("Expected expression.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
        }

        private (Expression, Expression, Expression) ParseOptionals(ParseContext parseContext)
        {
            Expression dateEnd = null;
            if (parseContext.MoveNextIfNextToken(TokenType.ENDS))
            {
                dateEnd = this.ParseExpressionWith<DateEndParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return (Expression.None, Expression.None, Expression.None);
                }
            }

            Expression condition = null;
            if (parseContext.MoveNextIfNextToken(TokenType.APPLY))
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
                if (!parseContext.MoveNextIfNextToken(TokenType.PRIORITY))
                {
                    parseContext.EnterPanicMode("Expected token 'PRIORITY'.", parseContext.GetCurrentToken());
                    return (Expression.None, Expression.None, Expression.None);
                }

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