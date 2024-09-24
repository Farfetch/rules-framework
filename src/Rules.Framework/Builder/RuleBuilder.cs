namespace Rules.Framework.Builder
{
    using System;
    using System.Linq;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Serialization;

    internal sealed class RuleBuilder : IRuleBuilder
    {
        private readonly RuleValidator ruleValidator = RuleValidator.Instance;

        private bool? active;

        private ContentContainer contentContainer;

        private string contentType;

        private DateTime dateBegin;

        private DateTime? dateEnd;

        private string name;

        private IConditionNode rootCondition;

        public RuleBuilderResult Build()
        {
            var rule = new Rule
            {
                ContentContainer = this.contentContainer,
                ContentType = this.contentType,
                DateBegin = this.dateBegin,
                DateEnd = this.dateEnd,
                Name = this.name,
                RootCondition = this.rootCondition,
                Active = this.active ?? true,
            };

            var validationResult = this.ruleValidator.Validate(rule);

            if (validationResult.IsValid)
            {
                return RuleBuilderResult.Success(rule);
            }

            return RuleBuilderResult.Failure(validationResult.Errors.Select(ve => ve.ErrorMessage).ToList());
        }

        public IRuleBuilder WithActive(bool active)
        {
            this.active = active;

            return this;
        }

        public IRuleBuilder WithCondition(IConditionNode condition)
        {
            this.rootCondition = condition;

            return this;
        }

        public IRuleBuilder WithCondition(
            Func<IRootConditionNodeBuilder, IConditionNode> conditionFunc)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder();

            var condition = conditionFunc.Invoke(rootConditionNodeBuilder);

            return this.WithCondition(condition);
        }

        public IRuleBuilder WithCondition<TDataType>(
            string conditionType, Operators condOperator, TDataType operand)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder();

            var valueCondition = rootConditionNodeBuilder.Value(conditionType, condOperator, operand);
            return this.WithCondition(valueCondition);
        }

        public IRuleBuilder WithContent(string contentType, object content)
        {
            this.contentType = contentType;
            this.contentContainer = new ContentContainer(_ => content);

            return this;
        }

        public IRuleBuilder WithDateBegin(DateTime dateBegin)
        {
            this.dateBegin = dateBegin;

            return this;
        }

        public IRuleBuilder WithDatesInterval(DateTime dateBegin, DateTime? dateEnd)
        {
            this.dateBegin = dateBegin;
            this.dateEnd = dateEnd;

            return this;
        }

        public IRuleBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public IRuleBuilder WithSerializedContent(
            string contentType,
            object serializedContent,
            IContentSerializationProvider contentSerializationProvider)
        {
            if (contentSerializationProvider is null)
            {
                throw new ArgumentNullException(nameof(contentSerializationProvider));
            }

            this.contentType = contentType;
            this.contentContainer = new SerializedContentContainer(contentType, serializedContent, contentSerializationProvider);

            return this;
        }
    }
}