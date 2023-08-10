namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class DeactivationParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public DeactivationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.DEACTIVATE))
            {
                throw new InvalidOperationException("Unable to handle deactivation rule statement.");
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
                parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetNextToken());
                return Expression.None;
            }

            var contentType = this.ParseExpressionWith<ContentTypeParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new DeactivationExpression(ruleName, contentType);
        }
    }
}