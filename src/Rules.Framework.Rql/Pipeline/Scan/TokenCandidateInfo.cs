namespace Rules.Framework.Rql.Pipeline.Scan
{
    using System;
    using Rules.Framework.Rql;

    internal class TokenCandidateInfo
    {
        public TokenCandidateInfo(uint startOffset, uint startLine, uint startColumn)
        {
            this.StartOffset = startOffset;
            this.EndOffset = startOffset;
            this.BeginPosition = RqlSourcePosition.From(startLine, startColumn);
            this.EndPosition = RqlSourcePosition.From(startLine, startColumn);
            this.HasError = false;
            this.Message = null;
        }

        public RqlSourcePosition BeginPosition { get; }
        public uint EndOffset { get; private set; }

        public RqlSourcePosition EndPosition { get; private set; }

        public bool HasError { get; private set; }

        public uint Length => this.EndOffset + 1 - this.StartOffset;

        public string Message { get; private set; }

        public uint StartOffset { get; }

        public void MarkAsError(string message)
        {
            if (this.HasError)
            {
                throw new InvalidOperationException("An error has already been reported for specified token candidate.");
            }

            this.HasError = true;
            this.Message = message;
        }

        public void NextColumn()
        {
            this.EndOffset++;
            this.EndPosition = RqlSourcePosition.From(this.EndPosition.Line, this.EndPosition.Column + 1);
        }

        public void NextLine()
        {
            this.EndOffset++;
            this.EndPosition = RqlSourcePosition.From(this.EndPosition.Line + 1, 1);
        }
    }
}