namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class ForEachParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public ForEachParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.FOREACH))
            {
                throw new InvalidOperationException("Unable to handle foreach statement.");
            }

            var forEachToken = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_LEFT))
            {
                parseContext.EnterPanicMode("Expected token '('.", parseContext.GetNextToken());
                return Statement.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.VAR))
            {
                parseContext.EnterPanicMode("Expected variable declaration.", parseContext.GetNextToken());
                return Statement.None;
            }

            var variableDeclaration = this.ParseExpressionWith<VariableDeclarationParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.IN))
            {
                parseContext.EnterPanicMode("Expected token 'in'.", parseContext.GetNextToken());
                return Statement.None;
            }

            var inToken = parseContext.GetCurrentToken();
            _ = parseContext.MoveNext();
            var sourceExpression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_RIGHT))
            {
                parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetNextToken());
                return Statement.None;
            }

            _ = parseContext.MoveNext();
            var forEachActionStatement = this.ParseStatementWith<StatementParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            return new ForEachStatement(forEachToken, variableDeclaration, inToken, sourceExpression, forEachActionStatement);
        }
    }
}