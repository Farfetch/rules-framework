namespace Rules.Framework.Rql.Expressions
{
    internal class ConditionGroupingExpression : Expression
    {
        public ConditionGroupingExpression(Expression rootCondition)
            : base(rootCondition.BeginPosition, rootCondition.EndPosition)
        {
            this.RootCondition = rootCondition;
        }

        public Expression RootCondition { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitConditionGroupingExpression(this);
    }
}