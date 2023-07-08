namespace Rules.Framework.Rql.Ast.Expressions
{
    internal class ValueConditionExpression : Expression
    {
        public ValueConditionExpression(
            Expression left,
            Expression @operator,
            Expression right)
            : base(left.BeginPosition, right.EndPosition)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        public Expression Left { get; }

        public Expression Operator { get; }

        public Expression Right { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitValueConditionExpression(this);
    }
}