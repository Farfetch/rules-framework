namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class RuleDefinitionParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public RuleDefinitionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            var definitionStatement = this.ParseDefinitionStatement(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            return new RuleDefinitionStatement(definitionStatement);
        }

        private Statement ParseDefinitionStatement(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.CREATE))
            {
                return this.ParseStatementWith<CreateRuleParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.UPDATE))
            {
                return this.ParseStatementWith<UpdateRuleParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ACTIVATE))
            {
                return this.ParseStatementWith<ActivationParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.DEACTIVATE))
            {
                return this.ParseStatementWith<DeactivationParseStrategy>(parseContext);
            }

            throw new InvalidOperationException("Unable to handle rule definition statement.");
        }
    }
}