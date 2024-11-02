namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class MatchExpression : Expression
    {
        private MatchExpression(
            Segment cardinality,
            Expression ruleset,
            Expression matchDate,
            Segment inputConditions)
            : base(cardinality.BeginPosition, inputConditions?.EndPosition ?? matchDate.EndPosition)
        {
            this.Cardinality = cardinality;
            this.InputConditions = inputConditions;
            this.MatchDate = matchDate;
            this.Ruleset = ruleset;
        }

        public Segment Cardinality { get; }

        public Segment InputConditions { get; }

        public Expression MatchDate { get; }

        public Expression Ruleset { get; }

        public static MatchExpression Create(Segment cardinality,
            Expression ruleset,
            Expression matchDate,
            Segment inputConditions)
            => new(cardinality, ruleset, matchDate, inputConditions);

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitMatchExpression(this);
    }
}