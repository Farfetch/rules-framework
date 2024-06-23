namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class NoneExpression : Expression
    {
        public NoneExpression()
            : base(RqlSourcePosition.Empty, RqlSourcePosition.Empty)
        {
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitNoneExpression(this);
    }
}