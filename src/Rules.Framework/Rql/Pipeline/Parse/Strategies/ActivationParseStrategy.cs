namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class ActivationParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public ActivationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.ACTIVATE))
            {
                throw new InvalidOperationException("Unable to handle activation rule statement.");
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

            return new ActivationStatement(ruleName, contentType);
        }
    }
}