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

        public bool IsEof() => this.IsEof(this.Offset);

        public bool IsMatchCurrentToken(params TokenType[] tokenTypes)
            => this.IsMatch(this.Offset, tokenTypes);

        public bool IsMatchNextToken(params TokenType[] tokenTypes)
            => this.IsMatch(this.Offset + 1, tokenTypes);

        public bool MoveNext()
            => this.Move(this.Offset + 1);

        public bool MoveNextIfCurrentToken(params TokenType[] tokenTypes)
        {
            if (this.IsMatchCurrentToken(tokenTypes))
            {
                return this.MoveNext();
            }

            return false;
        }

        public bool MoveNextIfNextToken(params TokenType[] tokenTypes)
        {
            if (this.IsMatchNextToken(tokenTypes))
            {
                return this.MoveNext();
            }

            return false;
        }

        private bool IsEof(int offset) => offset >= this.Tokens.Count || this.Tokens[offset].Type == TokenType.EOF;

        private bool IsMatch(int offset, params TokenType[] tokenTypes)
        {
            if (this.IsEof(offset))
            {
                return false;
            }

            foreach (var tokenType in tokenTypes)
            {
                if (this.Tokens[offset].Type == tokenType)
                {
                    return true;
                }
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