namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System.Collections.Generic;

    internal class ComposedConditionNodeDataModel<TConditionType> : ConditionNodeDataModel<TConditionType>
    {
        public IEnumerable<ConditionNodeDataModel<TConditionType>> ChildConditionNodes { get; set; }
    }
}