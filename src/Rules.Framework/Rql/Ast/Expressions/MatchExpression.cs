namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class MatchExpression : Expression
    {
        public MatchExpression(
            Segment cardinality,
            Expression contentType,
            Expression matchDate,
            Segment inputConditions)
            : base(cardinality.BeginPosition, inputConditions?.EndPosition ?? matchDate.EndPosition)
        {
            this.Cardinality = cardinality;
            this.ContentType = contentType;
            this.MatchDate = matchDate;
            this.InputConditions = inputConditions;
        }

        public Segment Cardinality { get; }

        public Expression ContentType { get; }

        public Segment InputConditions { get; }

        public Expression MatchDate { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitMatchExpression(this);
    }
}