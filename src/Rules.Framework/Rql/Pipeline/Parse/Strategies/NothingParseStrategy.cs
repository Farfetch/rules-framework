namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class NothingParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public NothingParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.NOTHING))
            {
                throw new InvalidOperationException("Unable to handle nothing expression.");
            }

            return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
        }
    }
}