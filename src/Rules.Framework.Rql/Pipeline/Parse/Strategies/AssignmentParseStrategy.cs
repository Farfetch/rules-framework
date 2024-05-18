namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class AssignmentParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public AssignmentParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            return this.ParseExpressionWith<RulesManipulationParseStrategy>(parseContext);

            // TODO: future logic to be added here for dealing with assignment of variables.
        }
    }
}