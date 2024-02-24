namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class AssignmentExpression : Expression
    {
        public AssignmentExpression(Expression left, Token assign, Expression right)
            : base(left.BeginPosition, right.EndPosition)
        {
            this.Left = left;
            this.Assign = assign;
            this.Right = right;
        }

        public Token Assign { get; }

        public Expression Left { get; }

        public Expression Right { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitAssignmentExpression(this);
    }
}