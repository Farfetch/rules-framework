namespace Rules.Framework.Rql.Pipeline.Parse
{
    using Rules.Framework.Rql.Tokens;

    internal readonly struct PanicModeInfo
    {
        public static readonly PanicModeInfo None = new PanicModeInfo(causeToken: null, message: null);

        public PanicModeInfo(Token causeToken, string message)
        {
            this.CauseToken = causeToken;
            this.Message = message;
        }

        public Token CauseToken { get; }

        public string Message { get; }
    }
}