namespace Rules.Framework.Rql.Statements
{
    internal abstract class Statement
    {
        protected Statement(RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public static Statement None { get; } = new NoneStatement();

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}