namespace Rules.Framework.Rql.Ast.Statements
{
    using Rules.Framework.Rql.Ast.Expressions;

    internal class ExpressionStatement : Statement
    {
        public ExpressionStatement(Expression expression)
            : base(expression.BeginPosition, expression.EndPosition)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitExpressionStatement(this);
    }
}