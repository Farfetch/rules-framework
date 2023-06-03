namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Tokens;

    internal class ParseContext
    {
        public ParseContext(IReadOnlyList<Token> tokens)
        {
            this.PanicMode = false;
            this.PanicModeInfo = PanicModeInfo.None;
            this.Tokens = tokens;
            this.Offset = -1;
        }

        public int Offset { get; private set; }

        public bool PanicMode { get; private set; }

        public PanicModeInfo PanicModeInfo { get; private set; }

        public IReadOnlyList<Token> Tokens { get; }

        public void EnterPanicMode(string infoMessage, Token causeToken)
        {
            if (this.PanicMode)
            {
                throw new InvalidOperationException("Parse operation is already in panic mode.");
            }

            this.PanicMode = true;
            this.PanicModeInfo = new PanicModeInfo(causeToken, infoMessage);
        }

        public void ExitPanicMode()
        {
            if (!this.PanicMode)
            {
                throw new InvalidOperationException("Parse operation is not in panic mode.");
            }

            this.PanicMode = false;
            this.PanicModeInfo = PanicModeInfo.None;
        }

        public Token GetCurrentToken()
        {
            if (this.Offset < 0)
            {
                throw new InvalidOperationException("Must invoke MoveNext() first.");
            }

            return this.Tokens[this.Offset];
        }

        public Token GetPreviousToken()
        {
            if (this.Offset < 0)
            {
                throw new InvalidOperationException("Must invoke MoveNext() first.");
            }

            if (this.Offset == 0)
            {
                throw new InvalidOperationException("Current offset does not have a previous token.");
            }

            return this.Tokens[this.Offset - 1];
        }

        public bool IsEof() => this.Tokens[this.Offset].Type == TokenType.EOF;

        public bool IsMatch(params TokenType[] tokenTypes)
        {
            if (this.IsEof())
            {
                return false;
            }

            foreach (var tokenType in tokenTypes)
            {
                if (this.Tokens[this.Offset].Type == tokenType)
                {
                    return true;
                }
            }

            return false;
        }

        public bool MoveNext()
            => this.Move(this.Offset + 1);

        public bool MoveNextConditionally(params TokenType[] tokenTypes)
        {
            if (this.IsMatch(tokenTypes))
            {
                return this.MoveNext();
            }

            return false;
        }

        private bool Move(int toOffset)
        {
            if (toOffset >= 0 && toOffset < this.Tokens.Count)
            {
                this.Offset = toOffset;
                return true;
            }

            return false;
        }
    }
}