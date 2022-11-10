namespace Rules.Framework.Generic
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a generic composed conidtion node
    /// </summary>
    /// <typeparam name="GenericConditionType">The type of the generic condition type.</typeparam>
    /// <seealso cref="Rules.Framework.Generic.GenericConditionNode&lt;GenericConditionType&gt;"/>
    public class GenericComposedConditionNode<GenericConditionType> : GenericConditionNode
    {
        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        public IEnumerable<GenericConditionNode> ChildConditionNodes { get; internal set; }
    }
}