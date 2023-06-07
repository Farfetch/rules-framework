namespace Rules.Framework.Rql.Statements
{
    using Rules.Framework.Rql.Expressions;

    internal class CreateStatement : Statement
    {
        public CreateStatement(
            Expression ruleName,
            Expression contentType,
            Expression content,
            Expression dateBegin,
            Expression dateEnd,
            Expression condition,
            Expression priorityOption)
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

        public Expression Condition { get; }

        public Expression Content { get; }

        public Expression ContentType { get; }

        public Expression DateBegin { get; }

        public Expression DateEnd { get; }

        public Expression PriorityOption { get; }

        public Expression RuleName { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitCreateStatement(this);
    }
}