namespace Rules.Framework.Rql.Ast.Statements
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;

    [ExcludeFromCodeCoverage]
    internal class ExpressionStatement : Statement
    {
        private ExpressionStatement(Expression expression, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
            : base(beginPosition, endPosition)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; }

        public static ExpressionStatement Create(Expression expression, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
            => new(expression, beginPosition, endPosition);

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitExpressionStatement(this);
    }
}