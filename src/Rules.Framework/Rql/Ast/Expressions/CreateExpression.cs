namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Segments;

    [ExcludeFromCodeCoverage]
    internal class CreateExpression : Expression
    {
        public CreateExpression(
            Expression ruleName,
            Expression contentType,
            Expression content,
            Expression dateBegin,
            Expression dateEnd,
            Segment condition,
            Segment priorityOption)
            : base(ruleName.BeginPosition, priorityOption?.EndPosition
                    ?? condition?.EndPosition
                    ?? dateEnd?.EndPosition
                    ?? dateBegin.EndPosition)
        {
            this.RuleName = ruleName;
            this.ContentType = contentType;
            this.Content = content;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.Condition = condition;
            this.PriorityOption = priorityOption;
        }

        public Segment? Condition { get; }

        public Expression Content { get; }

        public Expression ContentType { get; }

        public Expression DateBegin { get; }

        public Expression? DateEnd { get; }

        public Segment? PriorityOption { get; }

        public Expression RuleName { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCreateExpression(this);
    }
}