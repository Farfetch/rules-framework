namespace Rules.Framework.Generics
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines generic condition node
    /// </summary>
    /// <seealso cref="Rules.Framework.Generics.GenericConditionNode"/>
    public sealed class GenericComposedConditionNode : GenericConditionNode
    {
        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        public IEnumerable<GenericConditionNode> ChildConditionNodes { get; internal set; }
    }
}