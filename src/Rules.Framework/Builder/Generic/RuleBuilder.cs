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
        private readonly RuleBuilder ruleBuilder;

        public RuleBuilder()
        {
            this.ruleBuilder = new RuleBuilder();
        }

        public RuleBuilderResult<TContentType, TConditionType> Build()
        {
            var ruleBuilderResult = this.ruleBuilder.Build();

            if (ruleBuilderResult.IsSuccess)
            {
                var genericRule = new Rule<TContentType, TConditionType>(ruleBuilderResult.Rule);
                var validationResult = this.genericRuleValidator.Validate(genericRule);
                if (validationResult.IsValid)
                {
                    return RuleBuilderResult<TContentType, TConditionType>.Success(genericRule);
                }

                return RuleBuilderResult<TContentType, TConditionType>.Failure(validationResult.Errors.Select(ve => ve.ErrorMessage).ToList());
            }

            return RuleBuilderResult<TContentType, TConditionType>.Failure(ruleBuilderResult.Errors);
        }

        public IRuleBuilder<TContentType, TConditionType> WithActive(bool active)
        {
            this.ruleBuilder.WithActive(active);

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithCondition(IConditionNode condition)
        {
            this.ruleBuilder.WithCondition(condition);

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
            this.ruleBuilder.WithContent(GenericConversions.Convert(contentType), content);

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithDateBegin(DateTime dateBegin)
        {
            this.ruleBuilder.WithDateBegin(dateBegin);

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithDatesInterval(DateTime dateBegin, DateTime? dateEnd)
        {
            this.ruleBuilder.WithDatesInterval(dateBegin, dateEnd);

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithName(string name)
        {
            this.ruleBuilder.WithName(name);

            return this;
        }

        public IRuleBuilder<TContentType, TConditionType> WithSerializedContent(
            TContentType contentType,
            object serializedContent,
            IContentSerializationProvider contentSerializationProvider)
        {
            var contentTypeAsString = GenericConversions.Convert(contentType);
            this.ruleBuilder.WithSerializedContent(contentTypeAsString, serializedContent, contentSerializationProvider);

            return this;
        }
    }
}