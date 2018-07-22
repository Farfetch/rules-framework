using System;
using System.Collections.Generic;
using Rules.Framework.Core.ConditionNodes;

namespace Rules.Framework.Evaluation.ValueEvaluation
{
    internal interface IDeferredEval
    {
        Func<IEnumerable<Condition<TConditionType>>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode);
    }
}