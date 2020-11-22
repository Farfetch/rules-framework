namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using Rules.Framework.Core;

    internal class StubConditionNode<TConditionNode> : IConditionNode<TConditionNode>
    {
        public LogicalOperators LogicalOperator => throw new NotImplementedException();

        public IConditionNode<TConditionNode> Clone() => throw new NotImplementedException();
    }
}