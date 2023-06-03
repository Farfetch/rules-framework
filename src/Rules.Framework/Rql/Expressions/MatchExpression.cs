namespace Rules.Framework.Rql.Expressions
{
    using System.Collections.Generic;
    using System.Linq;

    internal class MatchExpression : Expression
    {
        public MatchExpression(
            Expression cardinality,
            Expression contentType,
            Expression matchDate,
            IEnumerable<Expression> inputConditions)
            : base(cardinality.BeginPosition, inputConditions.LastOrDefault()?.EndPosition ?? matchDate.EndPosition)
        {
            this.Cardinality = cardinality;
            this.ContentType = contentType;
            this.MatchDate = matchDate;
            this.InputConditions = inputConditions;
        }

        public Expression Cardinality { get; }

        public Expression ContentType { get; }

        public IEnumerable<Expression> InputConditions { get; set; }

        public Expression MatchDate { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitMatchExpression(this);
    }
}