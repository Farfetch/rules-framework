namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class SearchExpression : Expression
    {
        public SearchExpression(
            Expression searchKeyword,
            Expression rulesKeyword,
            Segment ruleset,
            Segment datesInterval,
            Segment inputConditions)
            : base(ruleset.BeginPosition, inputConditions?.EndPosition ?? datesInterval.EndPosition)
        {
            this.DatesInterval = datesInterval;
            this.InputConditions = inputConditions;
            this.SearchKeyword = searchKeyword;
            this.RulesKeyword = rulesKeyword;
            this.Ruleset = ruleset;
        }

        public Segment DatesInterval { get; }

        public Segment InputConditions { get; }

        public Segment Ruleset { get; }

        public Expression RulesKeyword { get; }

        public Expression SearchKeyword { get; }

        public static SearchExpression Create(
            Expression searchKeyword,
            Expression rulesKeyword,
            Segment ruleset,
            Segment datesInterval,
            Segment inputConditions) => new SearchExpression(searchKeyword, rulesKeyword, ruleset, datesInterval, inputConditions);

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitSearchExpression(this);
    }
}