namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal readonly struct PanicModeInfo : IEquatable<PanicModeInfo>
    {
        public static readonly PanicModeInfo None = new(causeToken: null!, message: null!);

        public PanicModeInfo(Token causeToken, string message)
        {
            this.CauseToken = causeToken;
            this.Message = message;
        }

        public Token CauseToken { get; }

        public string Message { get; }

        public bool Equals(PanicModeInfo other)
        {
            return this.CauseToken == other.CauseToken && string.Equals(this.Message, other.Message, StringComparison.Ordinal);
        }
    }
}