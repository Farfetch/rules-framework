namespace Rules.Framework.Rql.Pipeline.Scan
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Tokens;

    internal class ScanResult
    {
        private ScanResult(bool success, IReadOnlyList<Message> messages, IReadOnlyList<Token> tokens)
        {
            this.Success = success;
            this.Messages = messages;
            this.Tokens = tokens;
        }

        public IReadOnlyList<Message> Messages { get; }

        public bool Success { get; }

        public IReadOnlyList<Token> Tokens { get; }

        public static ScanResult CreateError(IReadOnlyList<Message> messages)
            => new ScanResult(success: false, messages, tokens: null);

        public static ScanResult CreateSuccess(IReadOnlyList<Token> tokens, IReadOnlyList<Message> messages)
            => new ScanResult(success: true, messages, tokens);
    }
}