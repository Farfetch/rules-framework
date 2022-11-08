namespace Rules.Framework.Generic
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic composed condition node
    /// </summary>
    /// <typeparam name="ConditionType">The type of the ondition type.</typeparam>
    /// <seealso cref="Rules.Framework.Generic.GenericConditionNode&lt;ConditionType&gt;"/>
    public class GenericComposedConditionNode<ConditionType> : GenericConditionNode<ConditionType>
    {
        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        public IEnumerable<GenericConditionNode<ConditionType>> ChildConditionNodes { get; internal set; }
    }
}