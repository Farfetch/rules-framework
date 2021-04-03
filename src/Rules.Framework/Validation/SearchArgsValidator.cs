namespace Rules.Framework.Validation
{
    using System;
    using FluentValidation;

    internal class SearchArgsValidator<TContentType, TConditionType> : AbstractValidator<SearchArgs<TContentType, TConditionType>>
    {
        private readonly Type conditionTypeRuntimeType;
        private readonly Type contentTypeRuntimeType;

        public SearchArgsValidator()
        {
            this.conditionTypeRuntimeType = typeof(TConditionType);
            this.contentTypeRuntimeType = typeof(TContentType);

            this.RuleFor(searchArgs => searchArgs.ContentType).Must(ct =>
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