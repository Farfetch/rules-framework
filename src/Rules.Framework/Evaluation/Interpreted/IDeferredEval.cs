namespace Rules.Framework.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core.ConditionNodes;

    internal interface IDeferredEval
    {
        Func<IDictionary<TConditionType, object>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode);
    }
}