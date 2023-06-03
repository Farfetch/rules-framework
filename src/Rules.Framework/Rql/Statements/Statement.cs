namespace Rules.Framework.Rql.Statements
{
    internal abstract class Statement
    {
        public static Statement None { get; } = new NoneStatement();

        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}