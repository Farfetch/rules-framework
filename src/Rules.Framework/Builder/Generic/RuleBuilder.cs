namespace Rules.Framework.Builder.Generic
{
    using System;
    using System.Linq;
    using Rules.Framework;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Generic;
    using Rules.Framework.Serialization;

    internal sealed class RuleBuilder<TContentType, TConditionType> : IRuleBuilder<TContentType, TConditionType>
    {
        private readonly GenericRuleValidator<TContentType, TConditionType> genericRuleValidator = GenericRuleValidator<TContentType, TConditionType>.Instance;
        private readonly RuleValidator ruleValidator = RuleValidator.Instance;
        private bool? active;
        private ContentContainer contentContainer;
        private DateTime dateBegin;
        private DateTime? dateEnd;
        private string name;
        private IConditionNode rootCondition;

        public RuleBuilderResult<TContentType, TConditionType> Build()
        {
            var rule = new Rule
            {
                ContentContainer = this.contentContainer,
                DateBegin = this.dateBegin,
                DateEnd = this.dateEnd,
                Name = this.name,
                RootCondition = this.rootCondition,
                Active = this.active ?? true,
            };

            var validationResult = this.ruleValidator.Validate(rule);

            if (validationResult.IsValid)
            {
                var genericRule = new Rule<TContentType, TConditionType>(rule);
                validationResult = this.genericRuleValidator.Validate(genericRule);
                if (validationResult.IsValid)
                {
                    return RuleBuilderResult<TContentType, TConditionType>.Success(genericRule);
                }
            }

            return RuleBuilderResult<TContentType, TConditionType>.Failure(validationResult.Errors.Select(ve => ve.ErrorMessage).ToList());
        }

        public IRuleBuilder<TContentType, TConditionType> WithActive(bool active)
        {
            this.active = active;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(IConditionNode condition)
        {
            this.rootCondition = condition;

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(
            Func<IRootConditionNodeBuilder<TConditionType>, IConditionNode> conditionFunc)
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
            var contentTypeAsString = GenericConversions.Convert(contentType);
            this.contentContainer = new ContentContainer(contentTypeAsString, _ => content);

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
            IContentSerializationProvider contentSerializationProvider)
        {
            if (contentSerializationProvider is null)
            {
                throw new ArgumentNullException(nameof(contentSerializationProvider));
            }

            var contentTypeAsString = GenericConversions.Convert(contentType);
            this.contentContainer = new SerializedContentContainer(contentTypeAsString, serializedContent, contentSerializationProvider);

            return this;
        }
    }
}