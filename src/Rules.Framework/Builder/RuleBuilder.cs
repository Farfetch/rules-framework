namespace Rules.Framework.Builder
{
    using System;
    using System.Linq;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Core;
    using Rules.Framework.Serialization;

    internal sealed class RuleBuilder<TContentType, TConditionType> : IRuleBuilder<TContentType, TConditionType>
    {
        private readonly int? priority;
        private readonly RuleValidator<TContentType, TConditionType> ruleValidator = RuleValidator<TContentType, TConditionType>.Instance;
        private bool? active;
        private ContentContainer<TContentType> contentContainer;
        private DateTime dateBegin;
        private DateTime? dateEnd;
        private string name;
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
                Active = this.active ?? true,
            };

            var validationResult = this.ruleValidator.Validate(rule);

            if (validationResult.IsValid)
            {
                return RuleBuilderResult.Success(rule);
            }

            return RuleBuilderResult.Failure<TContentType, TConditionType>(validationResult.Errors.Select(ve => ve.ErrorMessage).ToList());
        }

        public IRuleBuilder<TContentType, TConditionType> WithActive(bool active)
        {
            this.active = active;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(IConditionNode<TConditionType> condition)
        {
            this.rootCondition = condition;

            return this;
        }

        [Obsolete("This way of adding conditions is being deprecated. Please use a non-deprecated overload instead.")]
        public IRuleBuilder<TContentType, TConditionType> WithCondition(
            Func<IConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc)
        {
            var conditionNodeBuilder = new ConditionNodeBuilder<TConditionType>();

            var condition = conditionFunc.Invoke(conditionNodeBuilder);

            return this.WithCondition(condition);
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(
            Func<IRootConditionNodeBuilder<TConditionType>, IConditionNode<TConditionType>> conditionFunc)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder<TConditionType>();

            var condition = conditionFunc.Invoke(rootConditionNodeBuilder);

            return this.WithCondition(condition);
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition<TDataType>(
            TConditionType conditionType, Operators condOperator, TDataType operand)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder<TConditionType>();

            var valueCondition = rootConditionNodeBuilder.Value(conditionType, condOperator, operand);

            return this.WithCondition(valueCondition);
        }

        public IRuleBuilder<TContentType, TConditionType> WithContent(TContentType contentType, object content)
        {
            this.contentContainer = new ContentContainer<TContentType>(contentType, _ => content);

            return this;
        }

        [Obsolete("This way of building the content is being deprecated. Please use WithContent().")]
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

        public IRuleBuilder<TContentType, TConditionType> WithSerializedContent(
            TContentType contentType,
            object serializedContent,
            IContentSerializationProvider<TContentType> contentSerializationProvider)
        {
            if (contentSerializationProvider is null)
            {
                throw new ArgumentNullException(nameof(contentSerializationProvider));
            }

            this.contentContainer = new SerializedContentContainer<TContentType>(contentType, serializedContent, contentSerializationProvider);

            return this;
        }
    }
}