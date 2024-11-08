namespace Rules.Framework.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.ConditionNodes;

    internal interface IDeferredEval
    {
        Func<IDictionary<string, object>, bool> GetDeferredEvalFor(IValueConditionNode valueConditionNode, MatchModes matchMode);
    }
}