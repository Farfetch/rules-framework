namespace Rules.Framework.Rql.Messages
{
    internal class Message
    {
        private Message(string text, RqlSourcePosition beginPosition, RqlSourcePosition endPosition, MessageSeverity severity)
        {
            this.Text = text;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
            this.Severity = severity;
        }

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public MessageSeverity Severity { get; }

        public string Text { get; }

        public static Message Create(string text, RqlSourcePosition beginPosition, RqlSourcePosition endPosition, MessageSeverity severity)
            => new Message(text, beginPosition, endPosition, severity);
    }
}