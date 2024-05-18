namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            => this.GetToken(this.Offset);

        public Token GetNextToken()
        {
            if (this.Offset + 1 >= this.Tokens.Count)
            {
                return this.GetToken(this.Tokens.Count - 1);
            }

            return this.GetToken(this.Offset + 1);
        }

        public bool IsEof() => this.IsEof(this.Offset);

        public bool IsMatchAtOffsetFromCurrent(int offsetFromCurrent, params TokenType[] tokenTypes)
            => this.IsMatch(this.Offset + offsetFromCurrent, tokenTypes);

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

        private Token GetToken(int offset)
        {
            if (offset < 0)
            {
                throw new InvalidOperationException("Must invoke MoveNext() first.");
            }

            return this.Tokens[offset];
        }

        private bool IsEof(int offset) => offset >= this.Tokens.Count || this.Tokens[offset].Type == TokenType.EOF;

        private bool IsMatch(int offset, params TokenType[] tokenTypes)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be zero or greater.");
            }

            if (this.IsEof(offset))
            {
                if (tokenTypes.Contains(TokenType.EOF))
                {
                    return true;
                }

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
            if (toOffset < this.Tokens.Count)
            {
                this.Offset = toOffset;
                return true;
            }

            return false;
        }
    }
}