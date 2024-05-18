namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;

    internal class KeywordParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public KeywordParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var keywordToken = parseContext.GetCurrentToken();
            return KeywordExpression.Create(keywordToken);
        }
    }
}