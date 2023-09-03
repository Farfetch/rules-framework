namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Pipeline.Parse.Strategies;
    using Rules.Framework.Rql.Tokens;

    internal class Parser : IParser
    {
        private readonly IParseStrategyProvider parseStrategyProvider;

        public Parser(IParseStrategyProvider parseStrategyProvider)
        {
            this.parseStrategyProvider = parseStrategyProvider;
        }

        public ParseResult Parse(IReadOnlyList<Token> tokens)
        {
            var parseContext = new ParseContext(tokens);
            var statements = new List<Statement>();

            using var messageContainer = new MessageContainer();
            while (parseContext.MoveNext())
            {
                var statement = this.parseStrategyProvider.GetStatementParseStrategy<DeclarationParseStrategy>().Parse(parseContext);
                if (parseContext.PanicMode)
                {
                    var panicModeInfo = parseContext.PanicModeInfo;
                    messageContainer.Error(
                        panicModeInfo.Message,
                        panicModeInfo.CauseToken.BeginPosition,
                        panicModeInfo.CauseToken.EndPosition);
                    Synchronize(parseContext);
                    parseContext.ExitPanicMode();
                }
                else
                {
                    statements.Add(statement);
                }
            }

            var messages = messageContainer.Messages;
            if (messageContainer.ErrorsCount > 0)
            {
                return ParseResult.CreateError(messages);
            }

            return ParseResult.CreateSuccess(statements, messages);
        }

        private static void Synchronize(ParseContext parseContext)
        {
            while (parseContext.MoveNext())
            {
                switch (parseContext.GetCurrentToken().Type)
                {
                    case TokenType.SEMICOLON:
                    case TokenType.EOF:
                        return;

                    default:
                        break;
                }
            }
        }
    }
}