namespace Rules.Framework.Rql.Expressions
{
    using System.Collections.Generic;
    using System.Linq;

    internal class SearchExpression : Expression
    {
        public SearchExpression(Expression contentType,
            Expression dateBegin,
            Expression dateEnd,
            IEnumerable<Expression> inputConditions)
            : base(contentType.BeginPosition, inputConditions.LastOrDefault()?.EndPosition ?? dateEnd.EndPosition)
        {
            this.ContentType = contentType;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.InputConditions = inputConditions;
        }

        public Expression ContentType { get; }

        public Expression DateBegin { get; }

        public Expression DateEnd { get; }

        public IEnumerable<Expression> InputConditions { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitSearchExpression(this);
    }
}