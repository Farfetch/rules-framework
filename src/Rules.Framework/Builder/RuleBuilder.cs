namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Core;

    internal class RuleBuilder<TContentType, TConditionType> : IRuleBuilder<TContentType, TConditionType>
    {
        private string name;
        private DateTime dateBegin;
        private DateTime? dateEnd;
        private int priority;
        private ContentContainer<TContentType> contentContainer;
        private IConditionNode<TConditionType> rootCondition;

        public IRuleBuilder<TContentType, TConditionType> WithName(string name)
        {
            this.name = name;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithDateBegin(DateTime dateBegin)
        {
            this.dateBegin = dateBegin;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithDatesInterval(DateTime dateBegin, DateTime? dateEnd)
        {
            this.dateBegin = dateBegin;
            this.dateEnd = dateEnd;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(IConditionNode<TConditionType> condition)
        {
            this.rootCondition = condition;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc)
        {
            ConditionNodeBuilder<TConditionType> conditionNodeBuilder = new ConditionNodeBuilder<TConditionType>();

            IConditionNode<TConditionType> condition = conditionFunc.Invoke(conditionNodeBuilder);

            return this.WithCondition(condition);
        }

        public IRuleBuilder<TContentType, TConditionType> WithPriority(int priority)
        {
            this.priority = priority;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithContentContainer(ContentContainer<TContentType> contentContainer)
        {
            this.contentContainer = contentContainer;

            return this;
        }

        public Rule<TContentType, TConditionType> Build()
            => new Rule<TContentType, TConditionType>
            {
                ContentContainer = this.contentContainer,
                DateBegin = this.dateBegin,
                DateEnd = this.dateEnd,
                Name = this.name,
                Priority = this.priority,
                RootCondition = this.rootCondition
            };
    }
}
