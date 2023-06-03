namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Statements;

    internal class ParseResult
    {
        private ParseResult(bool success, IReadOnlyList<Message> messages, IReadOnlyList<Statement> statements)
        {
            this.Success = success;
            this.Messages = messages;
            this.Statements = statements;
        }

        public IReadOnlyList<Message> Messages { get; }

        public IReadOnlyList<Statement> Statements { get; }

        public bool Success { get; }

        public static ParseResult CreateError(IReadOnlyList<Message> messages)
            => new ParseResult(success: false, messages, statements: null);

        public static ParseResult CreateSuccess(IReadOnlyList<Statement> statements, IReadOnlyList<Message> messages)
            => new ParseResult(success: true, messages, statements);
    }
}