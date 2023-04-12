namespace Rules.Framework.Builder
{
    using System;
    using System.Linq;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Core;

    internal sealed class RuleBuilder<TContentType, TConditionType> : IRuleBuilder<TContentType, TConditionType>
    {
        private readonly RuleValidator<TContentType, TConditionType> ruleValidator = RuleValidator<TContentType, TConditionType>.Instance;

        private bool active;
        private ContentContainer<TContentType> contentContainer;
        private DateTime dateBegin;
        private DateTime? dateEnd;
        private string name;
        private int? priority;
        private IConditionNode<TConditionType> rootCondition;

        public RuleBuilderResult<TContentType, TConditionType> Build()
        {
            var rule = new Rule<TContentType, TConditionType>
            {
                ContentContainer = this.contentContainer,
                DateBegin = this.dateBegin,
                DateEnd = this.dateEnd,
                Name = this.name,
                Priority = this.priority.GetValueOrDefault(0),
                RootCondition = this.rootCondition,
                Active = this.active
            };

            var validationResult = this.ruleValidator.Validate(rule);

            if (validationResult.IsValid)
            {
                return RuleBuilderResult.Success(rule);
            }

            return RuleBuilderResult.Failure<TContentType, TConditionType>(validationResult.Errors.Select(ve => ve.ErrorMessage).ToList());
        }

        public IRuleBuilder<TContentType, TConditionType> WithActive(bool? active)
        {
            this.active = active ?? true;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(IConditionNode<TConditionType> condition)
        {
            this.rootCondition = condition;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc)
        {
            var conditionNodeBuilder = new ConditionNodeBuilder<TConditionType>();

            var condition = conditionFunc.Invoke(conditionNodeBuilder);

            return this.WithCondition(condition);
        }

        public IRuleBuilder<TContentType, TConditionType> WithContentContainer(ContentContainer<TContentType> contentContainer)
        {
            this.contentContainer = contentContainer;

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

        public IRuleBuilder<TContentType, TConditionType> WithName(string name)
        {
            this.name = name;

            return this;
        }
    }
}