namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class BlockParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public BlockParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_LEFT))
            {
                throw new InvalidOperationException("Unable to handle block statement.");
            }

            var braceLeft = parseContext.GetCurrentToken();
            var statements = new List<Statement>();
            while (parseContext.MoveNext() && !parseContext.IsMatchCurrentToken(TokenType.BRACE_RIGHT))
            {
                var statement = this.ParseStatementWith<DeclarationParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                statements.Add(statement);
            }

            if (!parseContext.IsMatchCurrentToken(TokenType.BRACE_RIGHT))
            {
                parseContext.EnterPanicMode("Expected closing '}' token for block statement.", parseContext.GetCurrentToken());
                return Statement.None;
            }

            var braceRight = parseContext.GetCurrentToken();
            return new BlockStatement(braceLeft, statements.ToArray(), braceRight);
        }
    }
}