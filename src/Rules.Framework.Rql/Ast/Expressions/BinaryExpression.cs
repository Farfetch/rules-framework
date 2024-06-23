namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class BinaryExpression : Expression
    {
        public BinaryExpression(Expression leftExpression, Segment operatorSegment, Expression rightExpression)
            : base(leftExpression.BeginPosition, rightExpression.EndPosition)
        {
            this.LeftExpression = leftExpression;
            this.OperatorSegment = operatorSegment;
            this.RightExpression = rightExpression;
        }

        public Expression LeftExpression { get; }

        public Segment OperatorSegment { get; }

        public Expression RightExpression { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBinaryExpression(this);
    }
}