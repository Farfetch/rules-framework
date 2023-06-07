namespace Rules.Framework.Rql.Expressions
{
    using System.Linq;

    internal class InputConditionsExpression : Expression
    {
        public InputConditionsExpression(Expression[] inputConditions)
            : base(inputConditions.FirstOrDefault()?.BeginPosition ?? RqlSourcePosition.Empty, inputConditions.LastOrDefault()?.EndPosition ?? RqlSourcePosition.Empty)
        {
            this.InputConditions = inputConditions;
        }

        public Expression[] InputConditions { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitInputConditionsExpression(this);
    }
}