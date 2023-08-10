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
            if (!parseContext.MoveNextIfNextToken(Constants.AllowedUnescapedIdentifierNames))
            {
                var nextToken = parseContext.GetNextToken();
                if (!nextToken.IsEscaped || !parseContext.IsMatchCurrentToken(Constants.AllowedEscapedIdentifierNames))
                {
                    parseContext.EnterPanicMode("Expected variable identifier.", nextToken);
                    return Expression.None;
                }
            }

            var variableIdentifier = this.ParseExpressionWith<IdentifierParseStrategy>(parseContext);
            return new VariableDeclarationExpression(keywordToken, variableIdentifier);
        }
    }
}