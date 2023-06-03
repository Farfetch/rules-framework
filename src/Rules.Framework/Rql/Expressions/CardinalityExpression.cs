namespace Rules.Framework.Rql.Expressions
{
    internal class CardinalityExpression : Expression
    {
        public CardinalityExpression(Expression cardinalityKeyword, Expression ruleKeyword)
            : base(cardinalityKeyword.BeginPosition, ruleKeyword.EndPosition)
        {
            this.CardinalityKeyword = cardinalityKeyword;
            this.RuleKeyword = ruleKeyword;
        }

        public Expression CardinalityKeyword { get; }

        public Expression RuleKeyword { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCardinalityExpression(this);
    }
}