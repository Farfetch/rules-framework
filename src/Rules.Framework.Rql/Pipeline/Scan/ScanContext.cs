namespace Rules.Framework.Rql.Pipeline.Scan
{
    using System;

    internal class ScanContext
    {
        private readonly string source;
        private int sourceColumn;
        private int sourceLine;

        public ScanContext(string source)
        {
            this.Offset = 0;
            this.sourceColumn = 0;
            this.sourceLine = 1;
            this.source = source;
        }

        public int Offset { get; private set; }

        public TokenCandidateInfo TokenCandidate { get; private set; }

        public IDisposable BeginTokenCandidate()
        {
            if (this.TokenCandidate is not null)
            {
                throw new InvalidOperationException("A token candidate is currently created. Cannot begin a new one.");
            }

            this.TokenCandidate = new TokenCandidateInfo((uint)this.Offset, (uint)this.sourceLine, (uint)this.sourceColumn);

            return new TokenCandidateScope(this);
        }

        public string ExtractLexeme()
        {
            if (this.TokenCandidate is null)
            {
                throw new InvalidOperationException("Must be on a token candidate scope. Ensure you have invoked" +
                    $" {nameof(BeginTokenCandidate)}() and extract lexeme before disposing of token candidate.");
            }

            return this.source.Substring((int)this.TokenCandidate.StartOffset, (int)(this.TokenCandidate.EndOffset - this.TokenCandidate.StartOffset + 1));
        }

        public char GetCurrentChar()
        {
            return this.source[this.Offset];
        }

        public char GetNextChar()
        {
            int nextOffset = this.Offset + 1;
            if (nextOffset >= this.source.Length)
            {
                return '\0';
            }

            return this.source[nextOffset];
        }

        public bool IsEof() => this.Offset >= this.source.Length - 1;

        public bool MoveNext()
            => this.Move(this.Offset + 1);

        public bool MoveNextConditionally(char expected)
        {
            var nextOffset = this.Offset + 1;

            if (nextOffset >= this.source.Length)
            {
                return false;
            }

            if (this.source[nextOffset] != expected)
            {
                return false;
            }

            return this.Move(nextOffset);
        }

        private void DiscardTokenCandidate()
        {
            this.TokenCandidate = null;
        }

        private bool Move(int offset)
        {
            if (offset >= 0 && offset < this.source.Length)
            {
                var toChar = this.source[offset];
                if (toChar == '\n')
                {
                    this.NextLine();
                }
                else
                {
                    this.NextColumn();
                }

                this.Offset = offset;
                return true;
            }

            return false;
        }

        private void NextColumn()
        {
            this.sourceColumn++;
            if (this.TokenCandidate is not null)
            {
                this.TokenCandidate.NextColumn();
            }
        }

        private void NextLine()
        {
            this.sourceLine++;
            this.sourceColumn = 1;
            if (this.TokenCandidate is not null)
            {
                this.TokenCandidate.NextLine();
            }
        }

        private class TokenCandidateScope : IDisposable
        {
            private readonly ScanContext scanContext;
            private bool disposed;

            public TokenCandidateScope(ScanContext scanContext)
            {
                this.scanContext = scanContext;
            }

            public void Dispose()
            {
                if (!this.disposed)
                {
                    this.scanContext.DiscardTokenCandidate();
                    this.disposed = true;
                }
            }
        }
    }
}