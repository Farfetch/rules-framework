namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class GreaterThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!args.DataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.GreaterThan))
            {
                throw new NotSupportedException($"The operator '{Operators.GreaterThan}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.GreaterThan(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}