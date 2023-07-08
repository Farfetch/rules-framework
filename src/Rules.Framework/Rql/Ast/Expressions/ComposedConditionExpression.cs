namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Linq;

    internal class ComposedConditionExpression : Expression
    {
        public ComposedConditionExpression(
            Expression logicalOperator,
            Expression[] childConditions)
            : base(childConditions.First().BeginPosition, childConditions.Last().EndPosition)
        {
            this.LogicalOperator = logicalOperator;
            this.ChildConditions = childConditions;
        }

        public Expression[] ChildConditions { get; }

        public Expression LogicalOperator { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitComposedConditionExpression(this);
    }
}