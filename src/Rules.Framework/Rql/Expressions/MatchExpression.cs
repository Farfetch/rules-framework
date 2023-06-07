namespace Rules.Framework.Rql.Expressions
{
    internal class MatchExpression : Expression
    {
        public MatchExpression(
            Expression cardinality,
            Expression contentType,
            Expression matchDate,
            Expression inputConditions)
            : base(cardinality.BeginPosition, inputConditions?.EndPosition ?? matchDate.EndPosition)
        {
            this.Cardinality = cardinality;
            this.ContentType = contentType;
            this.MatchDate = matchDate;
            this.InputConditions = inputConditions;
        }

        public Expression Cardinality { get; }

        public Expression ContentType { get; }

        public Expression InputConditions { get; }

        public Expression MatchDate { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitMatchExpression(this);
    }
}