namespace Rules.Framework.Rql.Ast.Expressions
{
    using Rules.Framework.Rql;

    internal abstract class Expression
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