namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class VariableDeclarationParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public VariableDeclarationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.VAR))
            {
                throw new InvalidOperationException("Unable to handle variable declaration statement.");
            }

            var keywordToken = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.IDENTIFIER))
            {
                parseContext.EnterPanicMode("Expected variable identifier.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var variableIdentifier = this.ParseExpressionWith<IdentifierParseStrategy>(parseContext);
            return new VariableDeclarationExpression(keywordToken, variableIdentifier);
        }
    }
}