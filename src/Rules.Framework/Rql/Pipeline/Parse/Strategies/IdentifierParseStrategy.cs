namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;

    internal class IdentifierParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public IdentifierParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var identifierToken = parseContext.GetCurrentToken();
            return new IdentifierExpression(identifierToken);
        }
    }
}