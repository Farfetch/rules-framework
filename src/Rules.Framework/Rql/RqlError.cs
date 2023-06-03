namespace Rules.Framework.Rql
{
    public class RqlError
    {
        public RqlError(string text, string rql, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.Text = text;
            this.Rql = rql;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public string Rql { get; }

        public string Text { get; }
    }
}