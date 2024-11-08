namespace Rules.Framework.Evaluation
{
    using System.Collections.Generic;

    internal interface IConditionsTreeAnalyzer
    {
        bool AreAllSearchConditionsPresent(IConditionNode conditionNode, IDictionary<string, object> conditions);
    }
}