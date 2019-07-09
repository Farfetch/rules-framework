namespace Rules.Framework.Evaluation
{
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal interface IConditionsEvalEngine<TConditionType>
    {
        bool Eval(IConditionNode<TConditionType> conditionNode, IEnumerable<Condition<TConditionType>> conditions);
    }
}