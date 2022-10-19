namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal interface IConditionsTreeCompiler<TConditionType>
    {
        void Compile(IConditionNode<TConditionType> conditionNode);
    }
}
