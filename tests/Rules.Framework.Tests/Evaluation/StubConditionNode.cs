namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;

    internal class StubConditionNode : IConditionNode
    {
        public LogicalOperators LogicalOperator => throw new NotImplementedException();

        public IDictionary<string, object> Properties => throw new NotImplementedException();

        public IConditionNode Clone() => throw new NotImplementedException();
    }
}