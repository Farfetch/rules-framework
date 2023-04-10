<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/IDeferredEval.cs
namespace Rules.Framework.Evaluation.Classic
========
namespace Rules.Framework.Evaluation.Interpreted
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/IDeferredEval.cs
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core.ConditionNodes;

    internal interface IDeferredEval
    {
        Func<IDictionary<TConditionType, object>, bool> GetDeferredEvalFor<TConditionType>(IValueConditionNode<TConditionType> valueConditionNode, MatchModes matchMode);
    }
}