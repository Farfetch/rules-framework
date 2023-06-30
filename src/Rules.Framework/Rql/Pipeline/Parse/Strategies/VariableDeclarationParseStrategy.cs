namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class VariableDeclarationParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public VariableDeclarationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.VAR))
            {
                throw new InvalidOperationException("Unable to handle variable declaration statement.");
            }

            var keywordToken = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.IDENTIFIER))
            {
                parseContext.EnterPanicMode("Expected variable identifier.", parseContext.GetCurrentToken());
                return Statement.None;
            }

            var variableIdentifier = this.ParseExpressionWith<IdentifierParseStrategy>(parseContext);
            Expression assignable;
            if (parseContext.MoveNextIfNextToken(TokenType.ASSIGN))
            {
                _ = parseContext.MoveNext();
                assignable = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }
            }
            else
            {
                assignable = Expression.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.SEMICOLON))
            {
                parseContext.EnterPanicMode("Expected token ';'.", parseContext.GetCurrentToken());
                return Statement.None;
            }

            return new VariableDeclarationStatement(keywordToken, variableIdentifier, assignable);
        }
    }
}