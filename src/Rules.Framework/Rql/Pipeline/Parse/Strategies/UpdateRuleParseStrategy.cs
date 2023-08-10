namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class UpdateRuleParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public UpdateRuleParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.UPDATE))
            {
                throw new InvalidOperationException("Unable to handle update rule statement.");
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

            if (!parseContext.MoveNextIfNextToken(TokenType.SET))
            {
                parseContext.EnterPanicMode("Expected token 'SET'.", parseContext.GetNextToken());
                return Expression.None;
            }

            var updatableAttribute = this.ParseSegmentWith<UpdatableAttributeParseStrategy>(parseContext);
            var updatableAttributes = new List<Segment> { updatableAttribute };
            while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.SET))
                {
                    parseContext.EnterPanicMode("Expected token 'SET'.", parseContext.GetNextToken());
                    return Expression.None;
                }

                updatableAttribute = this.ParseSegmentWith<UpdatableAttributeParseStrategy>(parseContext);
                updatableAttributes.Add(updatableAttribute);
            }

            return new UpdateExpression(ruleName, contentType, updatableAttributes.ToArray());
        }
    }
}