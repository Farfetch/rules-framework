namespace Rules.Framework.Rql.Expressions
{
    internal class PriorityOptionExpression : Expression
    {
        public PriorityOptionExpression(
            Expression priorityOption,
            Expression argument)
            : base(priorityOption.BeginPosition, argument?.EndPosition ?? priorityOption.EndPosition)
        {
            this.PriorityOption = priorityOption;
            this.Argument = argument;
        }

        public Expression Argument { get; }

        public Expression PriorityOption { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPriorityOptionExpression(this);
    }
}