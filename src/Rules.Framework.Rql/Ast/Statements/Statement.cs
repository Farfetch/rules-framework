namespace Rules.Framework.Rql.Ast.Statements
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql;

    [ExcludeFromCodeCoverage]
    internal abstract class Statement : IAstElement
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

        public bool ContainsPosition(RqlSourcePosition position)
            => this.BeginPosition <= position && this.EndPosition >= RqlSourcePosition.From(position.Line, position.Column - 1);
    }
}