namespace Rules.Framework.Evaluation
{
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal interface IConditionsTreeAnalyzer<TConditionType>
    {
        bool AreAllSearchConditionsPresent(IConditionNode<TConditionType> conditionNode, IDictionary<TConditionType, object> conditions);
    }
}