namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class IfParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public IfParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.IF))
            {
                throw new InvalidOperationException("Unable to handle if statement.");
            }

            var ifKeyword = parseContext.GetCurrentToken();
            if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_LEFT))
            {
                parseContext.EnterPanicMode("Expected token '('.", parseContext.GetNextToken());
                return Statement.None;
            }

            _ = parseContext.MoveNext();
            var conditionExpression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.BRACKET_RIGHT))
            {
                parseContext.EnterPanicMode("Expected token ')'.", parseContext.GetNextToken());
                return Statement.None;
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected statement after 'if' condition.", parseContext.GetNextToken());
                return Statement.None;
            }

            var thenBranchStatement = this.ParseStatementWith<StatementParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Statement.None;
            }

            if (parseContext.MoveNextIfNextToken(TokenType.ELSE))
            {
                if (!parseContext.MoveNext())
                {
                    parseContext.EnterPanicMode("Expected statement after 'else' keyword.", parseContext.GetNextToken());
                    return Statement.None;
                }

                var elseBranchStatement = this.ParseStatementWith<StatementParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new IfStatement(ifKeyword, conditionExpression, thenBranchStatement, elseBranchStatement);
            }

            return new IfStatement(ifKeyword, conditionExpression, thenBranchStatement, Statement.None);
        }
    }
}