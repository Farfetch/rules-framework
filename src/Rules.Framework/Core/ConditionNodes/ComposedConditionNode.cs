using System.Collections.Generic;

namespace Rules.Framework.Core.ConditionNodes
{
    public class ComposedConditionNode<TConditionNode> : IConditionNode<TConditionNode>
    {
        public ComposedConditionNode(LogicalOperators logicalOperator, IEnumerable<IConditionNode<TConditionNode>> childConditionNodes)
        {
            this.LogicalOperator = logicalOperator;
            this.ChildConditionNodes = childConditionNodes;
        }

        public IEnumerable<IConditionNode<TConditionNode>> ChildConditionNodes { get; }

        public LogicalOperators LogicalOperator { get; }
    }
}