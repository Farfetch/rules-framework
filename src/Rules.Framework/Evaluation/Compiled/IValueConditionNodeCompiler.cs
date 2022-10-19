namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core.ConditionNodes;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    internal interface IValueConditionNodeCompiler
    {
        Func<IDictionary<TConditionType, object>, bool> Compile<TConditionType>(
            ValueConditionNode<TConditionType> valueConditionNode,
            ParameterExpression parameterExpression);
    }
}
