namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ConditionsDefinitionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ConditionsDefinitionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.MoveNextIfCurrentToken(TokenType.APPLY))
            {
                throw new InvalidOperationException("Unable to handle conditions definition expression.");
            }

            if (!parseContext.MoveNextIfCurrentToken(TokenType.WHEN))
            {
                parseContext.EnterPanicMode("Expect token 'WHEN'.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            return this.ParseExpressionWith<ConditionGroupingParseStrategy>(parseContext);
        }
    }
}