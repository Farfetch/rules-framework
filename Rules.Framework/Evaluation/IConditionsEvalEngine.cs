using System.Collections.Generic;
using Rules.Framework.Core;

namespace Rules.Framework.Evaluation
{
    internal interface IConditionsEvalEngine<TConditionType>
    {
        bool Eval(IConditionNode<TConditionType> conditionNode, IEnumerable<Condition<TConditionType>> conditions);
    }
}