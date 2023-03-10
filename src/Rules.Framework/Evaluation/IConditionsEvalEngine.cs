namespace Rules.Framework.Evaluation
{
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal interface IConditionsEvalEngine<TConditionType>
    {
        bool Eval(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions, EvaluationOptions evaluationOptions);
    }
}