namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class MatchExpression : Expression
    {
        private MatchExpression(
            Expression matchKeyword,
            Segment cardinality,
            Segment ruleset,
            Segment matchDate,
            Segment inputConditions)
            : base(matchKeyword.BeginPosition, inputConditions?.EndPosition ?? matchDate.EndPosition)
        {
            this.Cardinality = cardinality;
            this.InputConditions = inputConditions;
            this.MatchDate = matchDate;
            this.Ruleset = ruleset;
        }

        public Segment Cardinality { get; }

        public Segment InputConditions { get; }

        public Segment MatchDate { get; }

        public Expression MatchKeyword { get; }

        public Segment Ruleset { get; }

        public static MatchExpression Create(
            Expression matchKeyword,
            Segment cardinality,
            Segment ruleset,
            Segment matchDate,
            Segment inputConditions)
            => new(matchKeyword, cardinality, ruleset, matchDate, inputConditions);

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitMatchExpression(this);
    }
}