namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class SearchExpression : Expression
    {
        public SearchExpression(Expression contentType,
            Expression dateBegin,
            Expression dateEnd,
            Segment inputConditions)
            : base(contentType.BeginPosition, inputConditions?.EndPosition ?? dateEnd.EndPosition)
        {
            this.ContentType = contentType;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.InputConditions = inputConditions;
        }

        public Expression ContentType { get; }

        public Expression DateBegin { get; }

        public Expression DateEnd { get; }

        public Segment InputConditions { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitSearchExpression(this);
    }
}