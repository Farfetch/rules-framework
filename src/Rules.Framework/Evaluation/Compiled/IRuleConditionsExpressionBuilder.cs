namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Linq.Expressions;

    internal interface IRuleConditionsExpressionBuilder
    {
        Expression<Func<EvaluationContext, bool>> BuildExpression(IConditionNode rootConditionNode);
    }
}