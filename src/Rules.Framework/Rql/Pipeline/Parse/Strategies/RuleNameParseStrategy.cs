namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class RuleNameParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public RuleNameParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.RULE))
            {
                throw new InvalidOperationException("Unable to handle rule name expression.");
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.STRING))
            {
                parseContext.EnterPanicMode("Expected rule name.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
        }
    }
}