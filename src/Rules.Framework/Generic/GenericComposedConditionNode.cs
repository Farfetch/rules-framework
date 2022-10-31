namespace Rules.Framework.Generic
{
    using System.Collections.Generic;

    public class GenericComposedConditionNode<ConditionType> : GenericConditionNode<ConditionType>
    {
        public IEnumerable<GenericConditionNode<ConditionType>> ChildConditionNodes { get; set; }
    }
}