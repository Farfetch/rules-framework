namespace Rules.Framework.Generic
{
    using System.Collections.Generic;

    public class GenericComposedConditionNode<ConditionType> : GenericConditionNode<ConditionType>
    {
        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        public IEnumerable<GenericConditionNode<ConditionType>> ChildConditionNodes { get; internal set; }
    }
}