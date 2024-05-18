namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal abstract class Expression : IAstElement
    {
        protected Expression(RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public static Expression None { get; } = new NoneExpression();

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public abstract T Accept<T>(IExpressionVisitor<T> visitor);
    }
}