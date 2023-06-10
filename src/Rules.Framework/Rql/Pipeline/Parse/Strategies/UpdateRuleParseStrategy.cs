namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class UpdateRuleParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public UpdateRuleParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.UPDATE))
            {
                throw new InvalidOperationException("Unable to handle update rule statement.");
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

            if (!parseContext.MoveNextIfNextToken(TokenType.SET))
            {
                parseContext.EnterPanicMode("Expected token 'SET'.", parseContext.GetCurrentToken());
                return Statement.None;
            }

            var updatableAttribute = this.ParseExpressionWith<UpdatableAttributeParseStrategy>(parseContext);
            var updatableAttributes = new List<Expression> { updatableAttribute };
            while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
            {
                if (!parseContext.MoveNextIfNextToken(TokenType.SET))
                {
                    parseContext.EnterPanicMode("Expected token 'SET'.", parseContext.GetCurrentToken());
                    return Statement.None;
                }

                updatableAttribute = this.ParseExpressionWith<UpdatableAttributeParseStrategy>(parseContext);
                updatableAttributes.Add(updatableAttribute);
            }

            if (parseContext.IsMatchNextToken(TokenType.SEMICOLON))
            {
                _ = parseContext.MoveNext();
            }

            return new UpdateStatement(ruleName, contentType, updatableAttributes.ToArray());
        }
    }
}