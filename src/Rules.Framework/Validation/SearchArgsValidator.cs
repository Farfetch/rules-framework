namespace Rules.Framework.Validation
{
    using System;
    using FluentValidation;

    internal sealed class SearchArgsValidator<TRuleset, TCondition> : AbstractValidator<SearchArgs<TRuleset, TCondition>>
    {
        private readonly Type conditionRuntimeType;
        private readonly Type rulesetRuntimeType;

        public SearchArgsValidator()
        {
            this.conditionRuntimeType = typeof(TCondition);
            this.rulesetRuntimeType = typeof(TRuleset);

            this.RuleFor(searchArgs => searchArgs.Ruleset).Must(ct =>
            {
                if (this.rulesetRuntimeType.IsClass && ct is null)
                {
                    return false;
                }

                if (this.rulesetRuntimeType.IsEnum && !Enum.IsDefined(this.rulesetRuntimeType, ct))
                {
                    return false;
                }

                return true;
            });

            this.RuleFor(searchArgs => searchArgs.DateEnd)
                .GreaterThanOrEqualTo(sa => sa.DateBegin)
                .WithMessage(searchArgs => $"'{nameof(searchArgs)}.{nameof(searchArgs.DateEnd)}' must be greater or equal to '{nameof(searchArgs)}.{nameof(searchArgs.DateBegin)}'");

            this.RuleForEach(sa => sa.Conditions)
                .ChildRules(conditionValidator =>
                {
                    conditionValidator.RuleFor(condition => condition.Key)
                        .Must(conditionKey =>
                        {
                            if (this.conditionRuntimeType.IsClass && conditionKey is null)
                            {
                                return false;
                            }

                            if (this.conditionRuntimeType.IsEnum && !Enum.IsDefined(this.conditionRuntimeType, conditionKey))
                            {
                                return false;
                            }

                            return true;
                        });
                });
        }
    }
}