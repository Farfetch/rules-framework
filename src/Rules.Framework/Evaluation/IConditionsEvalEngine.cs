namespace Rules.Framework.Evaluation
{
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal interface IConditionsEvalEngine
    {
        bool Eval(IConditionNode conditionNode, IDictionary<string, object> conditions, EvaluationOptions evaluationOptions);
    }
}