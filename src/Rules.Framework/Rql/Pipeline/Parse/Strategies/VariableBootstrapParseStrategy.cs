namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class VariableBootstrapParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public VariableBootstrapParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.VAR))
            {
                throw new InvalidOperationException("Unable to handle variable declaration statement.");
            }

            var variableDeclarationExpression = this.ParseExpressionWith<VariableDeclarationParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

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
                parseContext.EnterPanicMode("Expected token ';'.", parseContext.GetNextToken());
                return Statement.None;
            }

            return new VariableBootstrapStatement(variableDeclarationExpression, assignable);
        }
    }
}