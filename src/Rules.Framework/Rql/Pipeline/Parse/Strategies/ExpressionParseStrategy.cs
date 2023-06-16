namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Expressions;

    internal class ExpressionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ExpressionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            return this.ParseExpressionWith<AssignmentParseStrategy>(parseContext);
        }
    }
}