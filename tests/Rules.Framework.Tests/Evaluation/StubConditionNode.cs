namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal class StubConditionNode<TConditionNode> : IConditionNode<TConditionNode>
    {
        public LogicalOperators LogicalOperator => throw new NotImplementedException();

        public IDictionary<string, object> Properties => throw new NotImplementedException();

        public IConditionNode<TConditionNode> Clone() => throw new NotImplementedException();
    }
}