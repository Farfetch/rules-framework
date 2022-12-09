namespace Rules.Framework.Builder.Validation
{
    using System.Collections.Generic;
    using FluentValidation;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal sealed class ValueConditionNodeValidator<TConditionType> : AbstractValidator<ValueConditionNode<TConditionType>>
    {
        public ValueConditionNodeValidator()
        {
            this.RuleFor(c => c.ConditionType)
                .NotEmpty()
                .IsInEnum()
                .When(c => c.ConditionType.GetType().IsEnum);

            this.RuleFor(c => c.DataType)
                .IsInEnum()
                .Equal(DataTypes.Integer).When(c => c.Operand is int)
                .Equal(DataTypes.String).When(c => c.Operand is string)
                .Equal(DataTypes.Decimal).When(c => c.Operand is decimal)
                .Equal(DataTypes.Boolean).When(c => c.Operand is bool)
                .Equal(DataTypes.ArrayInteger).When(c => c.Operand is IEnumerable<int>)
                .Equal(DataTypes.ArrayString).When(c => c.Operand is IEnumerable<string>)
                .Equal(DataTypes.ArrayDecimal).When(c => c.Operand is IEnumerable<decimal>)
                .Equal(DataTypes.ArrayBoolean).When(c => c.Operand is IEnumerable<bool>);

            this.RuleFor(c => c.Operator)
                .IsInEnum();

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual, Operators.Contains, Operators.NotContains, Operators.StartsWith, Operators.EndsWith, Operators.CaseInsensitiveStartsWith, Operators.CaseInsensitiveEndsWith, Operators.NotStartsWith, Operators.NotEndsWith)
                .When(c => c.DataType == DataTypes.String)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual)
                .When(c => c.DataType == DataTypes.Boolean)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual, Operators.GreaterThan, Operators.GreaterThanOrEqual, Operators.LesserThan, Operators.LesserThanOrEqual)
                .When(c => c.DataType == DataTypes.Integer)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual, Operators.GreaterThan, Operators.GreaterThanOrEqual, Operators.LesserThan, Operators.LesserThanOrEqual)
                .When(c => c.DataType == DataTypes.Decimal)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In)
                .When(c => c.DataType == DataTypes.ArrayString)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In)
                .When(c => c.DataType == DataTypes.ArrayInteger)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In)
                .When(c => c.DataType == DataTypes.ArrayDecimal)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In)
                .When(c => c.DataType == DataTypes.ArrayBoolean)
                .WithMessage(cn => $"Condition nodes with data type '{cn.DataType}' can't define a operator of type '{cn.Operator}'.");
        }
    }
}