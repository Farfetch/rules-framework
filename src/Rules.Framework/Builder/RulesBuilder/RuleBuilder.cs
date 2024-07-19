namespace Rules.Framework.Builder.RulesBuilder
{
    using System;
    using System.Linq;
    using Rules.Framework.Builder.Validation;
    using Rules.Framework.Serialization;

    internal sealed class RuleBuilder :
        IRuleBuilder,
        IRuleConfigureContent,
        IRuleConfigureDateBegin,
        IRuleConfigureDateEndOptional,
        IRuleConfigureRuleset
    {
        private readonly string name;
        private readonly RuleValidator ruleValidator = RuleValidator.Instance;

        private bool? active;
        private ContentContainer? contentContainer;
        private DateTime dateBegin;
        private DateTime? dateEnd;
        private IConditionNode? rootCondition;
        private string? ruleset;

        public RuleBuilder(string name)
        {
            this.name = name;
        }

        public IRuleBuilder ApplyWhen(IConditionNode condition)
        {
            this.rootCondition = condition;
            return this;
        }

        public IRuleBuilder ApplyWhen(Func<IRootConditionNodeBuilder, IConditionNode> conditionFunc)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder();
            var condition = conditionFunc.Invoke(rootConditionNodeBuilder);
            return this.ApplyWhen(condition);
        }

        public IRuleBuilder ApplyWhen<T>(string condition, Operators condOperator, T operand)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder();
            var valueCondition = rootConditionNodeBuilder.Value(condition, condOperator, operand);
            return this.ApplyWhen(valueCondition);
        }

        public RuleBuilderResult Build()
        {
            var rule = new Rule
            {
                Active = this.active ?? true,
                ContentContainer = this.contentContainer!,
                DateBegin = this.dateBegin,
                DateEnd = this.dateEnd,
                Name = this.name,
                RootCondition = this.rootCondition!,
                Ruleset = this.ruleset!,
            };

            var validationResult = this.ruleValidator.Validate(rule);

            if (validationResult.IsValid)
            {
                return RuleBuilderResult.Success(rule);
            }

            return RuleBuilderResult.Failure(validationResult.Errors.Select(ve => ve.ErrorMessage).ToList());
        }

        public IRuleConfigureContent OnRuleset(string ruleset)
        {
            this.ruleset = ruleset;
            return this;
        }

        public IRuleConfigureDateBegin SetContent(object content)
        {
            this.contentContainer = new ContentContainer(_ => content);
            return this;
        }

        public IRuleConfigureDateBegin SetContent(object content, IContentSerializationProvider contentSerializationProvider)
        {
            if (contentSerializationProvider is null)
            {
                throw new ArgumentNullException(nameof(contentSerializationProvider));
            }

            this.contentContainer = new SerializedContentContainer(this.ruleset!, content, contentSerializationProvider);
            return this;
        }

        public IRuleConfigureDateEndOptional Since(DateTime dateBegin)
        {
            this.dateBegin = dateBegin;
            return this;
        }

        public IRuleBuilder Until(DateTime? dateEnd)
        {
            this.dateEnd = dateEnd;
            return this;
        }

        public IRuleBuilder WithActive(bool active)
        {
            this.active = active;
            return this;
        }
    }
}