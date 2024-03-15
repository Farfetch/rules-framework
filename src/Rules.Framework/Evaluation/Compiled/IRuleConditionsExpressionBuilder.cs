namespace Rules.Framework.Evaluation.Compiled
{
    using System;
    using System.Linq.Expressions;
    using Rules.Framework.Core;

    internal interface IRuleConditionsExpressionBuilder<TConditionType>
    {
        Expression<Func<EvaluationContext<TConditionType>, bool>> BuildExpression(IConditionNode<TConditionType> rootConditionNode);
    }
}