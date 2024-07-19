namespace Rules.Framework.Validation
{
    using System;
    using FluentValidation;

    internal sealed class SearchArgsValidator<TRuleset, TCondition> : AbstractValidator<SearchArgs<TRuleset, TCondition>>
    {
        private readonly Type conditionTypeRuntimeType;
        private readonly Type contentTypeRuntimeType;

        public SearchArgsValidator()
        {
            this.conditionTypeRuntimeType = typeof(TCondition);
            this.contentTypeRuntimeType = typeof(TRuleset);

            this.RuleFor(searchArgs => searchArgs.Ruleset).Must(ct =>
            {
                if (this.contentTypeRuntimeType.IsClass && ct is null)
                {
                    return false;
                }

                if (this.contentTypeRuntimeType.IsEnum && !Enum.IsDefined(this.contentTypeRuntimeType, ct))
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
                    conditionValidator.RuleFor(condition => condition.Type)
                        .Must(conditionType =>
                        {
                            if (this.conditionTypeRuntimeType.IsClass && conditionType is null)
                            {
                                return false;
                            }

                            if (this.conditionTypeRuntimeType.IsEnum && !Enum.IsDefined(this.conditionTypeRuntimeType, conditionType))
                            {
                                return false;
                            }

                            return true;
                        });
                });
        }
    }
}