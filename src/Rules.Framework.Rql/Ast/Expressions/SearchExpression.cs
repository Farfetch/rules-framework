namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class SearchExpression : Expression
    {
        public SearchExpression(Expression ruleset,
            Expression dateBegin,
            Expression dateEnd,
            Segment inputConditions)
            : base(ruleset.BeginPosition, inputConditions?.EndPosition ?? dateEnd.EndPosition)
        {
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.InputConditions = inputConditions;
            this.Ruleset = ruleset;
        }

        public Expression DateBegin { get; }

        public Expression DateEnd { get; }

        public Segment InputConditions { get; }

        public Expression Ruleset { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitSearchExpression(this);
    }
}