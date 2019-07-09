namespace Rules.Framework.Core.ConditionNodes
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Composed condition: apply {LogicalOperator.ToString(),nq} operator for {System.Linq.Enumerable.Count(ChildConditionNodes),nq} nodes")]
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