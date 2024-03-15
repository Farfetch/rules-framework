namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class GreaterThanOrEqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!args.DataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.GreaterThanOrEqual))
            {
                throw new NotSupportedException($"The operator '{Operators.GreaterThanOrEqual}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.GreaterThanOrEqual(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}