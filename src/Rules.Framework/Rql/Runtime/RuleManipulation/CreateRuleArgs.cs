namespace Rules.Framework.Rql.Runtime.RuleManipulation
{
    using Rules.Framework.Core;
    using Rules.Framework.Rql.Runtime.Types;

    internal class CreateRuleArgs<TContentType, TConditionType>
    {
        private CreateRuleArgs(
            TContentType contentType,
            RqlString name,
            IRuntimeValue content,
            RqlDate dateBegin,
            RqlAny dateEnd,
            IConditionNode<TConditionType> condition,
            PriorityOption priorityOption)
        {
            this.ContentType = contentType;
            this.Name = name;
            this.Content = content;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.Condition = condition;
            this.PriorityOption = priorityOption;
        }

        public IConditionNode<TConditionType> Condition { get; }

        public IRuntimeValue Content { get; }

        public TContentType ContentType { get; }

        public RqlDate DateBegin { get; }

        public RqlAny DateEnd { get; }

        public RqlString Name { get; }

        public PriorityOption PriorityOption { get; }

        public static CreateRuleArgs<TContentType, TConditionType> Create(
            TContentType contentType,
            RqlString name,
            IRuntimeValue content,
            RqlDate dateBegin,
            RqlAny dateEnd,
            IConditionNode<TConditionType> condition,
            PriorityOption priorityOption)
            => new CreateRuleArgs<TContentType, TConditionType>(contentType, name, content, dateBegin, dateEnd, condition, priorityOption);
    }
}